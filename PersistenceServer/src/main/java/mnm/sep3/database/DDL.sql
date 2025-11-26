DROP SCHEMA IF EXISTS sep3_eksamen cascade;
CREATE SCHEMA sep3_eksamen;
SET SCHEMA 'sep3_eksamen';

CREATE TABLE Questions (
    id SERIAL PRIMARY KEY,
    question VARCHAR NOT NULL,
    answer VARCHAR NOT NULL
);

CREATE TABLE Users (
    id SERIAL PRIMARY KEY,
    username VARCHAR(16) UNIQUE NOT NULL,
    password VARCHAR NOT NULL
);
