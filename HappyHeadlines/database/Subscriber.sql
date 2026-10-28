CREATE TABLE IF NOT EXISTS "Subscribers"
(
    "Id" uuid PRIMARY KEY,

    "Email" varchar(320) NOT NULL UNIQUE,

    "CreatedAt" timestamp with time zone NOT NULL
);

CREATE INDEX IF NOT EXISTS "IX_Subscribers_CreatedAt"
    ON "Subscribers" ("CreatedAt" DESC);