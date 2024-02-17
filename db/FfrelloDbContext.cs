using FFrelloApi.Models;
using Microsoft.EntityFrameworkCore;

namespace FFrelloApi.database
{
    public class FfrelloDbContext : DbContext
    {
        public FfrelloDbContext() : base()
        {
            Database.EnsureCreated(); //this is to get rid of table does not exist error
        }

        public FfrelloDbContext(DbContextOptions<FfrelloDbContext> options)
            : base(options)
        {
            //Database.EnsureCreated(); //this is to get rid of table does not exist error
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<FFrelloRefreshToken>()
                .HasOne(x => x.User)
                .WithOne(t => t.RefreshToken)
                .HasForeignKey<User>(x => x.RefreshTokenId);

            modelBuilder.Entity<User>()
                .HasMany(w => w.Workspaces);

            modelBuilder.Entity<User>()
                .HasOne(w => w.RefreshToken)
                .WithOne(t => t.User)
                .HasForeignKey<FFrelloRefreshToken>(x => x.UserId);

            modelBuilder.Entity<Workspace>()
                .HasMany(w => w.Boards)
                .WithOne(b => b.Workspace)
                .HasForeignKey(b => b.WorkspaceId);

            modelBuilder.Entity<Workspace>()
                .HasIndex(w => w.Name)
                .IsUnique();

            modelBuilder.Entity<Workspace>()
                .HasOne(x => x.User)
                .WithMany(x => x.Workspaces)
                .HasForeignKey(y => y.UserId);

            modelBuilder.Entity<Board>()
                .HasMany(b => b.BoardLists)
                .WithOne(l => l.Board)
                .HasForeignKey(l => l.BoardId);

            modelBuilder.Entity<BoardList>()
                .HasMany(l => l.Cards)
                .WithOne(c => c.BoardList)
                .HasForeignKey(c => c.BoardListId);

            modelBuilder.Entity<Card>()
               .HasMany(l => l.Comments)
               .WithOne(c => c.Card)
               .HasForeignKey(c => c.CardId);

            //create composite key to ensure uniqueness
            modelBuilder.Entity<CardWatcher>()
                .HasKey(cw => new { cw.UserId, cw.CardId });

            modelBuilder.Entity<CardWatcher>()
                .HasOne(cw => cw.User)
                .WithMany() // No navigation property
                .HasForeignKey(cw => cw.UserId);

            modelBuilder.Entity<CardWatcher>()
                .HasOne(cw => cw.Card)
                .WithMany(c => c.CardWatchers)
                .HasForeignKey(cw => cw.CardId);

            modelBuilder.Entity<User>().HasData(
                new User()
                {
                    Id = 1,
                    Email = "yluksim9@gmail.com",
                }
            );

            modelBuilder.Entity<Card>().HasData(
                new Card()
                {
                    Id = 1,
                    BoardListId = 1,
                    BoardListName = "TODO",
                    Title = "Franks Card",
                    Description = "Franks Description of this card"
                },
                new Card()
                {
                    Id = 2,
                    BoardListId = 2,
                    BoardListName = "In Progress",
                    Title = "Franks 2nd Card",
                    Description = "Franks Description of the 2nd card"
                },
                new Card()
                {
                    Id = 3,
                    BoardListId = 3,
                    BoardListName = "DONE",
                    Title = "Franks 3rd Card",
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
                        IsStarred = true,
                    },
                    new Board()
                    {
                        Id = 3,
                        Name = "Philly Board",
                        WorkspaceId = 1,
                    },
                    new Board()
                    {
                        Id = 4,
                        Name = "Peaches Board",
                        WorkspaceId = 2,
                        IsStarred = true,
                    },
                    new Board()
                    {
                        Id = 5,
                        Name = "Painting Board",
                        WorkspaceId = 2,
                    },
                    new Board()
                    {
                        Id = 6,
                        Name = "Prill Board",
                        WorkspaceId = 3,
                    }
                );

            modelBuilder.Entity<Workspace>().HasData(
                new Workspace
                {
                    Id = 1,
                    Name = "Frank",
                    UserId = 1,
                },
                new Workspace
                {
                    Id = 2,
                    Name = "Catherine",
                    UserId = 1,
                },
                new Workspace
                {
                    Id = 3,
                    Name = "M.C.",
                    UserId = 1,
                }
            );
        }


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=test.db");
        }

        public DbSet<Workspace> Workspaces => Set<Workspace>();
        public DbSet<Board> Boards => Set<Board>();
        public DbSet<BoardList> BoardLists => Set<BoardList>();
        public DbSet<Card> Cards => Set<Card>();
        public DbSet<User> Users => Set<User>();
        public DbSet<CardWatcher> CardWatchers => Set<CardWatcher>();
        public DbSet<FFrelloRefreshToken> RefreshTokens => Set<FFrelloRefreshToken>();
        public DbSet<CardComment> CardComments => Set<CardComment>();
    }
}