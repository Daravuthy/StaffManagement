using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using StaffManagement.Api.Data;
using StaffManagement.Api.Models;
using StaffManagement.Api.Services;
using StaffManagement.Shared.Enums;
using StaffManagement.Shared.Requests;

namespace StaffManagement.UnitTests;

public sealed class StaffServiceTests
{
    [Fact]
    public async Task SearchAsync_FiltersByGenderAndBirthdayRange()
    {
        await using var dbContext = await CreateDbContextAsync();
        dbContext.Staffs.AddRange(
            new Staff
            {
                StaffId = "EMP00001",
                FullName = "John Doe",
                Birthday = new DateOnly(1990, 1, 10),
                Gender = Gender.Male
            },
            new Staff
            {
                StaffId = "EMP00002",
                FullName = "Jane Roe",
                Birthday = new DateOnly(1994, 5, 20),
                Gender = Gender.Female
            });
        await dbContext.SaveChangesAsync();

        var service = new StaffService(dbContext);

        var result = await service.SearchAsync(new StaffSearchRequest
        {
            Gender = Gender.Female,
            BirthdayFrom = new DateOnly(1994, 1, 1),
            BirthdayTo = new DateOnly(1994, 12, 31)
        }, CancellationToken.None);

        Assert.Single(result);
        Assert.Equal("EMP00002", result[0].StaffId);
    }

    [Fact]
    public async Task CreateAsync_RejectsDuplicateStaffId()
    {
        await using var dbContext = await CreateDbContextAsync();
        dbContext.Staffs.Add(new Staff
        {
            StaffId = "EMP00001",
            FullName = "Existing User",
            Birthday = new DateOnly(1991, 8, 1),
            Gender = Gender.Male
        });
        await dbContext.SaveChangesAsync();

        var service = new StaffService(dbContext);

        await Assert.ThrowsAsync<InvalidOperationException>(() => service.CreateAsync(new StaffUpsertRequest
        {
            StaffId = "EMP00001",
            FullName = "Another User",
            Birthday = new DateOnly(1992, 3, 1),
            Gender = Gender.Female
        }, CancellationToken.None));
    }

    [Fact]
    public async Task CreateAsync_RejectsFutureBirthday()
    {
        await using var dbContext = await CreateDbContextAsync();
        var service = new StaffService(dbContext);

        await Assert.ThrowsAsync<ArgumentException>(() => service.CreateAsync(new StaffUpsertRequest
        {
            StaffId = "EMP99999",
            FullName = "Future User",
            Birthday = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(2)),
            Gender = Gender.Male
        }, CancellationToken.None));
    }

    private static async Task<AppDbContext> CreateDbContextAsync()
    {
        var connection = new SqliteConnection("DataSource=:memory:");
        await connection.OpenAsync();

        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlite(connection)
            .Options;

        var dbContext = new AppDbContext(options);
        await dbContext.Database.EnsureCreatedAsync();
        return dbContext;
    }
}
