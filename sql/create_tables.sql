-- Create tablse for permafnotes

BEGIN TRANSACTION

DROP TABLE IF EXISTS notes_tags
DROP TABLE IF EXISTS tags
DROP TABLE IF EXISTS notes

CREATE TABLE notes (
    [id] bigint identity(1,1) not null,
    [title] nvarchar(MAX) COLLATE Japanese_90_CI_AS_SC_UTF8 not null,
    [reference] nvarchar(MAX) COLLATE Japanese_90_CI_AS_SC_UTF8 not null,
    [source] nvarchar(MAX) COLLATE Japanese_90_CI_AS_SC_UTF8 not null,
    [memo] nvarchar(MAX) COLLATE Japanese_90_CI_AS_SC_UTF8 not null,
    [created_at] DATETIMEOFFSET(3) not null,
    [updated_at] DATETIMEOFFSET(3)
    constraint PK_notes primary key (id),
    constraint CHK_notes_id_positive CHECK (id > 0)
);

CREATE TABLE tags (
    [id] bigint identity(1,1) not null,
    [name] nvarchar(MAX) COLLATE Japanese_90_CI_AS_SC_UTF8 not null,
    [description] nvarchar(MAX) COLLATE Japanese_90_CI_AS_SC_UTF8 not null,
    constraint PK_tags primary key (id),
    constraint CHK_tags_id_positive CHECK (id > 0)
);

CREATE TABLE notes_tags (
    [note_id] bigint not null,
    [tag_id] bigint not null,
    constraint PK_notes_tags primary key (note_id, tag_id),
    constraint FK_notes_tags_note_id foreign key (note_id) references notes(id),
    constraint FK_notes_tags_tag_id foreign key (tag_id) references tags(id),
    constraint CHK_notes_tags_note_id_positive CHECK (note_id > 0),
    constraint CHK_notes_tags_tag_id_positive CHECK (tag_id > 0)
);

COMMIT TRANSACTION;
