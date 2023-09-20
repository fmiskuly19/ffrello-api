using Microsoft.EntityFrameworkCore;
using System.Reflection.PortableExecutable;
using test.Models;

namespace test.database;

public class TestContext : DbContext
{
    public TestContext() : base()
    {
        Database.EnsureCreated(); //this is to get rid of table does not exist error
    }

    public TestContext(DbContextOptions<TestContext> options)
        : base(options)
    {
        Database.EnsureCreated(); //this is to get rid of table does not exist error
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        //modelBuilder.Entity<BoardList>().HasMany(e => e.Cards);
        //modelBuilder.Entity<Board>().HasMany(e => e.BoardLists);

        modelBuilder.Entity<Workspace>()
            .HasMany(w => w.Boards)
            .WithOne(b => b.Workspace)
            .HasForeignKey(b => b.WorkspaceId);

        modelBuilder.Entity<Board>()
            .HasMany(b => b.BoardLists)
            .WithOne(l => l.Board)
            .HasForeignKey(l => l.BoardId);

        modelBuilder.Entity<BoardList>()
            .HasMany(l => l.Cards)
            .WithOne(c => c.BoardList)
            .HasForeignKey(c => c.BoardListId);

        modelBuilder.Entity<Card>().HasData(
            new Card()
            {
                Id = 1,
                BoardListId = 1,
                Name = "Franks Card",
                Description = "Franks Description of this card"
            },
            new Card()
            {
                Id = 2,
                BoardListId = 2,
                Name = "Franks 2nd Card",
                Description = "Franks Description of the 2nd card"
            },
            new Card()
            {
                Id = 3,
                BoardListId = 3,
                Name = "Franks 3rd Card",
                Description = "Franks Description of the 3nd card"
            }
        );

        modelBuilder.Entity<BoardList>().HasData(
            new BoardList()
            {
                Id = 1,
                Name = "TODO",
                BoardId = 3,
            },
            new BoardList()
            {
                Id = 2,
                Name = "In Progress",
                BoardId = 3,
            },
            new BoardList()
            {
                Id = 3,
                Name = "DONE",
                BoardId = 3,
            }
        );


        modelBuilder.Entity<Board>().HasData(
                new Board()
                {
                    Id = 1,
                    Name = "Buffalo Board",
                    WorkspaceId = 1,
                },
                new Board()
                {
                    Id = 2,
                    Name = "Vermont Board",
                    WorkspaceId = 1,
                },
                new Board()
                {
                    Id = 3,
                    Name = "Philly Board",
                    WorkspaceId = 1,
                }
            );

        modelBuilder.Entity<Workspace>().HasData(
            new Workspace
            {
                Id = 1,
                Name = "Franks Workspace",
            }
        );
    }


    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite("Data Source=test.db");
    }

    public DbSet<Workspace> Workspaces => Set<Workspace>();
    public DbSet<Board> Boards => Set<Board>();
    public DbSet<Card> Cards => Set<Card>();
}