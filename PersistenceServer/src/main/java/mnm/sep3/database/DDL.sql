DROP SCHEMA IF EXISTS sep3_eksamen cascade;
CREATE SCHEMA sep3_eksamen;
SET SCHEMA 'sep3_eksamen';

ALTER SCHEMA sep3_eksamen OWNER TO sep3_eksamen;

ALTER DEFAULT PRIVILEGES IN SCHEMA sep3_eksamen
    GRANT ALL PRIVILEGES ON TABLES TO sep3_eksamen;

ALTER DEFAULT PRIVILEGES IN SCHEMA sep3_eksamen
    GRANT ALL PRIVILEGES ON SEQUENCES TO sep3_eksamen;

CREATE DOMAIN visibility AS VARCHAR NOT NULL CHECK (VALUE IN ('public', 'private'));

CREATE TABLE users (
    id SERIAL PRIMARY KEY,
    username VARCHAR(16) UNIQUE NOT NULL,
    password VARCHAR NOT NULL
);

CREATE TABLE quizzes (
    id SERIAL PRIMARY KEY,
    title VARCHAR NOT NULL,
    visibility visibility,
    creator_id INT REFERENCES users (id) ON DELETE SET NULL
);

CREATE TABLE questions (
    id SERIAL PRIMARY KEY,
    title VARCHAR NOT NULL,
    index INT NOT NULL,
    in_quiz_id INT NOT NULL REFERENCES quizzes (id) ON DELETE CASCADE
);

CREATE TABLE answers (
    id SERIAL PRIMARY KEY,
    title VARCHAR NOT NULL,
    index INT NOT NULL,
    is_correct BOOLEAN NOT NULL,
    question_id INT NOT NULL REFERENCES questions (id) ON DELETE CASCADE
);

-- Dummy data
-- Insert users
INSERT INTO users (username, password)
VALUES ('testuser', 'password123');

-- Insert quiz
INSERT INTO quizzes (title, visibility, creator_id)
VALUES ('General Knowledge Quiz', 'public', 1);

-- Insert 10 questions
INSERT INTO questions (title, index, in_quiz_id)
VALUES
    ('What is the capital of France?', 1, 1),
    ('Which planet is known as the Red Planet?', 2, 1),
    ('Who wrote "Romeo and Juliet"?', 3, 1),
    ('What is the largest ocean on Earth?', 4, 1),
    ('Which element has the chemical symbol O?', 5, 1),
    ('How many continents are there?', 6, 1),
    ('What is the square root of 64?', 7, 1),
    ('Which animal is known as the King of the Jungle?', 8, 1),
    ('What is the boiling point of water at sea level (°C)?', 9, 1),
    ('Which country invented pizza?', 10, 1);

-- Insert answers (4 per question)
INSERT INTO answers (title, index, is_correct, question_id) VALUES
-- Q1
('Paris', 1, TRUE, 1),
('London', 2, FALSE, 1),
('Berlin', 3, FALSE, 1),
('Madrid', 4, FALSE, 1),

-- Q2
('Mars', 1, TRUE, 2),
('Jupiter', 2, FALSE, 2),
('Saturn', 3, FALSE, 2),
('Venus', 4, FALSE, 2),

-- Q3
('William Shakespeare', 1, TRUE, 3),
('Charles Dickens', 2, FALSE, 3),
('Mark Twain', 3, FALSE, 3),
('J.K. Rowling', 4, FALSE, 3),

-- Q4
('Pacific Ocean', 1, TRUE, 4),
('Atlantic Ocean', 2, FALSE, 4),
('Indian Ocean', 3, FALSE, 4),
('Arctic Ocean', 4, FALSE, 4),

-- Q5
('Oxygen', 1, TRUE, 5),
('Gold', 2, FALSE, 5),
('Hydrogen', 3, FALSE, 5),
('Carbon', 4, FALSE, 5),

-- Q6
('7', 1, TRUE, 6),
('5', 2, FALSE, 6),
('6', 3, FALSE, 6),
('8', 4, FALSE, 6),

-- Q7
('8', 1, TRUE, 7),
('6', 2, FALSE, 7),
('10', 3, FALSE, 7),
('9', 4, FALSE, 7),

-- Q8
('Lion', 1, TRUE, 8),
('Tiger', 2, FALSE, 8),
('Elephant', 3, FALSE, 8),
('Leopard', 4, FALSE, 8),

-- Q9
('100', 1, TRUE, 9),
('80', 2, FALSE, 9),
('120', 3, FALSE, 9),
('90', 4, FALSE, 9),

-- Q10
('Italy', 1, TRUE, 10),
('France', 2, FALSE, 10),
('USA', 3, FALSE, 10),
('Germany', 4, FALSE, 10);
