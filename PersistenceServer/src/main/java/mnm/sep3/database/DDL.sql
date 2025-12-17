DROP SCHEMA IF EXISTS sep3_eksamen cascade;
CREATE SCHEMA sep3_eksamen;
SET SCHEMA 'sep3_eksamen';

ALTER SCHEMA sep3_eksamen OWNER TO sep3_eksamen;

ALTER DEFAULT PRIVILEGES IN SCHEMA sep3_eksamen
    GRANT ALL PRIVILEGES ON TABLES TO sep3_eksamen;

ALTER DEFAULT PRIVILEGES IN SCHEMA sep3_eksamen
    GRANT ALL PRIVILEGES ON SEQUENCES TO sep3_eksamen;

CREATE DOMAIN visibility AS VARCHAR NOT NULL CHECK (VALUE IN ('public', 'private'));
CREATE DOMAIN gameid AS VARCHAR(36);

CREATE TABLE users
(
    id       SERIAL PRIMARY KEY,
    username VARCHAR(16) UNIQUE NOT NULL,
    password VARCHAR            NOT NULL
);

CREATE TABLE quizzes
(
    id         SERIAL PRIMARY KEY,
    title      VARCHAR NOT NULL,
    visibility visibility,
    creator_id INT     REFERENCES users (id) ON DELETE SET NULL
);

CREATE TABLE questions
(
    id         SERIAL PRIMARY KEY,
    title      VARCHAR NOT NULL,
    index      INT     NOT NULL,
    in_quiz_id INT     NOT NULL REFERENCES quizzes (id) ON DELETE CASCADE
);

CREATE TABLE answers
(
    id          SERIAL PRIMARY KEY,
    title       VARCHAR NOT NULL,
    index       INT     NOT NULL,
    is_correct  BOOLEAN NOT NULL,
    question_id INT     NOT NULL REFERENCES questions (id) ON DELETE CASCADE
);


CREATE TABLE games
(
    game_id     gameid PRIMARY KEY,
    host_id     INT NOT NULL REFERENCES users (id),
    played_time BIGINT NOT NULL,
    quiz_id     INT NOT NULL REFERENCES quizzes (id) ON DELETE CASCADE
);

CREATE TABLE participants
(
    id          SERIAL PRIMARY KEY,
    name        VARCHAR NOT NULL,
    game_id     gameid NOT NULL REFERENCES games (game_id) ON DELETE CASCADE,
    score       INT NOT NULL
);

CREATE TABLE participant_answers
(
    id          SERIAL PRIMARY KEY,
    answer_id   INT NOT NULL REFERENCES answers (id) ON DELETE CASCADE,
    participant INT NOT NULL REFERENCES participants (id) ON DELETE CASCADE
);


-- Dummy data
-- Insert users
INSERT INTO users (username, password)
VALUES ('malthe', 'password123'),
       ('mazen', 'password123'),
       ('nikolai', 'password123'),
       ('bømanden', 'password123'),
       ('testuser', 'password123');

-- Insert quiz
INSERT INTO quizzes (title, visibility, creator_id)
VALUES ('Er du klogere end min ven?', 'public', 1),
       ('Hurtig quiz om alt muligt', 'private', 2),
       ('Random facts jeg lærte i går', 'public', 3),
       ('Test dine småkloge skills', 'public', 4),
       ('Min første hjemmebyggede quiz', 'private', 5),
       ('Gæt årstallet', 'public', 1),
       ('Mærkelige dyrefacts', 'private', 2),
       ('Sandt eller falsk – svære version', 'public', 3),
       ('Mini-quiz jeg lavede i toget', 'public', 4),
       ('Kan du slå min score?', 'private', 5),
       ('Dumme spørgsmål med dumme svar', 'public', 1),
       ('Alt om snacks', 'private', 2),
       ('Hverdagsviden for begyndere', 'public', 3),
       ('Nørdequiz om ingenting', 'public', 4),
       ('Ting jeg lærte fra TikTok', 'private', 5),
       ('Quizzer du bedre end mig?', 'public', 1),
       ('Random skoleviden', 'private', 2),
       ('Rigtigt svært mix', 'public', 3),
       ('Lidt af hvert quiz', 'public', 4),
       ('Kan du gætte det her?', 'private', 5),
       ('To-minutters udfordring', 'public', 1),
       ('Spørgsmål jeg fandt på 2 sekunder', 'private', 2),
       ('Virkelig mærkelig trivia', 'public', 3),
       ('Mit ultimative quiz-rod', 'public', 4),
       ('Gode gamle klassikere', 'private', 5),
       ('Mig mod dig', 'public', 1),
       ('Mini-geografi-test', 'private', 2),
       ('Hurtig filmrunde', 'public', 3),
       ('Hvad ved du egentlig?', 'public', 4),
       ('Test din hukommelse', 'private', 5),
       ('Små ting folk ofte glemmer', 'private', 1),
       ('Nørd eller ej?', 'private', 2),
       ('Fritidsquiz 3000', 'public', 3),
       ('Mit eget trivia-kaos', 'public', 4),
       ('Sære ting fra internettet', 'private', 5),
       ('Det mest random du ser i dag', 'public', 1),
       ('Hverdagslogik', 'private', 2),
       ('Spørgsmål uden mening', 'public', 3),
       ('Lidt for nem quiz', 'public', 4),
       ('Den store ingentingstest', 'private', 5),
       ('Sjove facts du ikke kendte', 'private', 1),
       ('Musik du burde kende', 'private', 2),
       ('Mit lille vidensmix', 'public', 3),
       ('Quiz lavet mens jeg ventede på pizza', 'public', 4),
       ('Gæt hvad jeg tænker på', 'private', 5),
       ('Hurtigt, simpelt og kaotisk', 'public', 1),
       ('Internettets bedste småfacts', 'private', 2),
       ('Spontan quiz', 'public', 3),
       ('Tre spørgsmål jeg lige fandt på', 'public', 4),
       ('Ganske almindelig quiz', 'private', 5),
       ('For sjov quiz', 'public', 1),
       ('10 ting jeg undrer mig over', 'private', 2),
       ('Mine yndlingsspørgsmål', 'public', 3),
       ('Hjemmelavet lynrunde', 'public', 4),
       ('Quiztime!', 'private', 5),
       ('Hurtig fyraftensquiz', 'public', 1),
       ('Diverse trivia', 'private', 2),
       ('Test din spontanviden', 'public', 3),
       ('Gæt det rigtige svar', 'public', 4),
       ('Uimponerende men sjov', 'private', 5),
       ('Endnu en quiz ingen bad om', 'public', 1),
       ('Blandet pose trivia', 'private', 2),
       ('Spørgsmål fra min vennechat', 'public', 3),
       ('Ting alle burde vide', 'public', 4),
       ('Virkelig basic quiz', 'private', 5),
       ('Spørgsmål jeg stjal fra min bror', 'public', 1),
       ('Kaotisk men god', 'private', 2),
       ('3 hurtige', 'public', 3),
       ('Til dig der keder dig', 'public', 4),
       ('Meget mærkeligt vidensmix', 'private', 5),
       ('Quiz lavet på under 1 minut', 'public', 1),
       ('Fun facts til weekenden', 'private', 2),
       ('Kan du svare på det her?', 'public', 3),
       ('Gæt temaet', 'public', 4),
       ('Små random udfordringer', 'private', 5),
       ('Alt mellem himmel og jord', 'public', 1),
       ('Quizzen uden tema', 'private', 2),
       ('Let men underholdende', 'public', 3),
       ('Hurtige hovedbrud', 'public', 4),
       ('Rundt om alt og intet', 'private', 5),
       ('Uden filter quiz', 'public', 1),
       ('Ugens lille udfordring', 'private', 2),
       ('Quiz på farten', 'public', 3),
       ('Test din intuition', 'public', 4),
       ('Kan du gætte rigtigt?', 'private', 5),
       ('Mit bedste bud på en quiz', 'public', 1),
       ('Blandet videnstest', 'private', 2),
       ('Sjov pausequiz', 'public', 3),
       ('Lynhurtig challenge', 'public', 4),
       ('Hyggequiz deluxe', 'private', 5);


-- Insert 10 questions
INSERT INTO questions (title, index, in_quiz_id)
VALUES ('What is the capital of France?', 1, 1),
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
INSERT INTO answers (title, index, is_correct, question_id)
VALUES
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
