use [SceneIt]
go

if col_length('dbo.Movies', 'IsDeleted') is null
begin
    alter table [dbo].[Movies]
    add [IsDeleted] bit not null
        constraint [DF_Movies_IsDeleted] default (0);
end
go

if col_length('dbo.Movies', 'DeletedAtUtc') is null
begin
    alter table [dbo].[Movies]
    add [DeletedAtUtc] datetime2 null;
end
go
