CREATE TABLE IF NOT EXISTS "Articles"
(
    "Id" uuid PRIMARY KEY,
    "Title" varchar(500) NOT NULL,
    "Content" text NOT NULL,
    "Author" varchar(200) NOT NULL,
    "CreatedAt" timestamp with time zone NOT NULL,
    "UpdatedAt" timestamp with time zone NOT NULL
);