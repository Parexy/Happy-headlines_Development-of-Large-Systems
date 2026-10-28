CREATE TABLE IF NOT EXISTS feature_flags
(
    name VARCHAR(100) PRIMARY KEY,
    enabled BOOLEAN NOT NULL DEFAULT FALSE
);

INSERT INTO feature_flags (name, enabled)
VALUES ('SubscriberService', FALSE)
ON CONFLICT (name) DO NOTHING;
