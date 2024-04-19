
    public class SampleAppContext
    {
        private readonly string _connectionString;

        public SampleAppContext(string connectionString)
        {
            _connectionString = connectionString;
        }

        public void CreateTables()
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = @"
CREATE TABLE [dbo].[Users]
(
    [Id] INT NOT NULL PRIMARY KEY IDENTITY(1,1),
    [Name] varchar(100) not null,
    [Email] varchar(100) not null,
    [Password] varchar(100) not null,
    [Password_Confirmation] varchar(100) not null,
    [IsAdmin] BIT NOT NULL default 0
)";
                    command.ExecuteNonQuery();

                    command.CommandText = @"
CREATE TABLE [dbo].[Microposts]
(
    [Id] INT NOT NULL PRIMARY KEY IDENTITY(1,1),
    [Content] Text NOT NULL,
    [UserId] INT NOT NULL,
    [CreatedAt] DateTime NOT NULL,
    [UpdatedAt] DateTime NOT NULL,
    CONSTRAINT [FK_Microposts_ToUsers] FOREIGN KEY ([UserId]) REFERENCES [Users]([Id])
)";
                    command.ExecuteNonQuery();

                    command.CommandText = @"
CREATE TABLE [dbo].[Relations]
(
    [Id] INT NOT NULL PRIMARY KEY IDENTITY(1,1),
    [FollowerId] int not null,
    [FollowedId] int not null,
    CONSTRAINT [Follower] FOREIGN KEY ([FollowerId]) REFERENCES [Users]([Id]),
    CONSTRAINT [Followed] FOREIGN KEY ([FollowedId]) REFERENCES [Users]([Id]),
    CONSTRAINT [UniqPairFollowedFollower] UNIQUE ([FollowerId],[FollowedId]),
    CONSTRAINT [SelfFollowerCheck] CHECK (FollowerId != FollowedId)
)";
                    command.ExecuteNonQuery();
                }
            }
        }
    }
