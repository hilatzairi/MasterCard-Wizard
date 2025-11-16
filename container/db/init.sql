CREATE DATABASE McWizardDB;
GO

USE McWizardDB;
GO

CREATE TABLE Organizations (
    Id INT PRIMARY KEY IDENTITY,
    Name NVARCHAR(255) NOT NULL,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE()
);

CREATE TABLE Environments (
    Id INT PRIMARY KEY IDENTITY,
    OrganizationId INT NOT NULL,
    Name NVARCHAR(255) NOT NULL,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    FOREIGN KEY (OrganizationId) REFERENCES Organizations(Id)
);

CREATE TABLE Questions (
    Code NVARCHAR(50) PRIMARY KEY,
    Text NVARCHAR(MAX) NOT NULL,
    Type NVARCHAR(50) NOT NULL
);

CREATE TABLE QuestionOptions (
    Id INT PRIMARY KEY IDENTITY,
    QuestionCode NVARCHAR(50) NOT NULL,
    Value NVARCHAR(255) NOT NULL,
    DisplayText NVARCHAR(255) NOT NULL,
    SortOrder INT NOT NULL DEFAULT 0,
    FOREIGN KEY (QuestionCode) REFERENCES Questions(Code)
);

CREATE TABLE BucketConfigurations (
    BucketName NVARCHAR(50) PRIMARY KEY,
    MinEnvironments INT NOT NULL,
    MaxEnvironments INT
);

CREATE TABLE NavigationRules (
    RuleId INT PRIMARY KEY IDENTITY,
    CurrentQuestionCode NVARCHAR(50) NOT NULL,
    AnswerValue NVARCHAR(255),
    NextQuestionCode NVARCHAR(50),
    RecommendedBucket NVARCHAR(50),
    ConditionType NVARCHAR(50),
    Priority INT NOT NULL DEFAULT 1,
    FOREIGN KEY (CurrentQuestionCode) REFERENCES Questions(Code),
    FOREIGN KEY (RecommendedBucket) REFERENCES BucketConfigurations(BucketName)
);

CREATE TABLE WizardSessions (
    Id UNIQUEIDENTIFIER PRIMARY KEY,
    OrganizationId INT NOT NULL,
    CurrentQuestionCode NVARCHAR(50),
    IsCompleted BIT NOT NULL,
    RecommendedBucket NVARCHAR(50),
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    CompletedAt DATETIME2,
    FOREIGN KEY (OrganizationId) REFERENCES Organizations(Id),
    FOREIGN KEY (RecommendedBucket) REFERENCES BucketConfigurations(BucketName)
);

CREATE TABLE SessionAnswers (
    Id INT PRIMARY KEY IDENTITY,
    SessionId UNIQUEIDENTIFIER NOT NULL,
    QuestionCode NVARCHAR(50) NOT NULL,
    Answer NVARCHAR(MAX) NOT NULL,
    AnsweredAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    FOREIGN KEY (SessionId) REFERENCES WizardSessions(Id)
);
GO 

INSERT INTO BucketConfigurations (BucketName, MinEnvironments, MaxEnvironments) VALUES ('Lite', 1, 1);
INSERT INTO BucketConfigurations (BucketName, MinEnvironments, MaxEnvironments) VALUES ('Medium', 2, 3);
INSERT INTO BucketConfigurations (BucketName, MinEnvironments, MaxEnvironments) VALUES ('Premium', 4, NULL);

INSERT INTO Questions (Code, Text, Type) VALUES ('START', 'Wizard Start', 'Internal');

INSERT INTO Questions (Code, Text, Type) VALUES ('OrgSize', 'What is your organization size?', 'SingleChoice');
INSERT INTO QuestionOptions (QuestionCode, Value, DisplayText, SortOrder) VALUES ('OrgSize', 'Small', 'Small', 1);
INSERT INTO QuestionOptions (QuestionCode, Value, DisplayText, SortOrder) VALUES ('OrgSize', 'Medium', 'Medium', 2);
INSERT INTO QuestionOptions (QuestionCode, Value, DisplayText, SortOrder) VALUES ('OrgSize', 'Large', 'Large', 3);

INSERT INTO Questions (Code, Text, Type) VALUES ('Startup', 'Are you a startup or early-stage company?', 'SingleChoice');
INSERT INTO QuestionOptions (QuestionCode, Value, DisplayText, SortOrder) VALUES ('Startup', 'Yes', 'Yes', 1);
INSERT INTO QuestionOptions (QuestionCode, Value, DisplayText, SortOrder) VALUES ('Startup', 'No', 'No', 2);

INSERT INTO Questions (Code, Text, Type) VALUES ('Coverage', 'How much of your organization do you want to cover in this assessment?', 'SingleChoice');
INSERT INTO QuestionOptions (QuestionCode, Value, DisplayText, SortOrder) VALUES ('Coverage', 'Only core systems', 'Only core systems', 1);
INSERT INTO QuestionOptions (QuestionCode, Value, DisplayText, SortOrder) VALUES ('Coverage', 'Most departments', 'Most departments', 2);
INSERT INTO QuestionOptions (QuestionCode, Value, DisplayText, SortOrder) VALUES ('Coverage', 'Full organization', 'Full organization', 3);

INSERT INTO Questions (Code, Text, Type) VALUES ('Environments', 'How many systems or environments do you plan to assess?', 'SingleChoice');
INSERT INTO QuestionOptions (QuestionCode, Value, DisplayText, SortOrder) VALUES ('Environments', '1', '1', 1);
INSERT INTO QuestionOptions (QuestionCode, Value, DisplayText, SortOrder) VALUES ('Environments', '2-3', '2-3', 2);
INSERT INTO QuestionOptions (QuestionCode, Value, DisplayText, SortOrder) VALUES ('Environments', '4-10', '4-10', 3);
INSERT INTO QuestionOptions (QuestionCode, Value, DisplayText, SortOrder) VALUES ('Environments', 'More than 10', 'More than 10', 4);

INSERT INTO Questions (Code, Text, Type) VALUES ('EnvSelection', 'Select the environments you want to assess', 'MultiChoice');

INSERT INTO NavigationRules (CurrentQuestionCode, AnswerValue, NextQuestionCode, ConditionType, Priority) VALUES ('START', NULL, 'OrgSize', NULL, 1);

INSERT INTO NavigationRules (CurrentQuestionCode, AnswerValue, NextQuestionCode, ConditionType, Priority) VALUES ('OrgSize', 'Small', 'Startup', NULL, 1);
INSERT INTO NavigationRules (CurrentQuestionCode, AnswerValue, NextQuestionCode, ConditionType, Priority) VALUES ('OrgSize', 'Medium', 'Coverage', NULL, 1);
INSERT INTO NavigationRules (CurrentQuestionCode, AnswerValue, NextQuestionCode, ConditionType, Priority) VALUES ('OrgSize', 'Large', 'EnvSelection', 'HasEnvironments', 1);
INSERT INTO NavigationRules (CurrentQuestionCode, AnswerValue, NextQuestionCode, ConditionType, Priority) VALUES ('OrgSize', 'Large', 'Environments', NULL, 2);

INSERT INTO NavigationRules (CurrentQuestionCode, AnswerValue, NextQuestionCode, ConditionType, Priority) VALUES ('Startup', 'Yes', 'EnvSelection', 'HasEnvironments', 1);
INSERT INTO NavigationRules (CurrentQuestionCode, AnswerValue, RecommendedBucket, ConditionType, Priority) VALUES ('Startup', 'Yes', 'Lite', NULL, 2);
INSERT INTO NavigationRules (CurrentQuestionCode, AnswerValue, NextQuestionCode, ConditionType, Priority) VALUES ('Startup', 'No', 'Coverage', NULL, 1);

INSERT INTO NavigationRules (CurrentQuestionCode, AnswerValue, NextQuestionCode, ConditionType, Priority) VALUES ('Coverage', 'Only core systems', 'EnvSelection', 'HasEnvironments', 1);
INSERT INTO NavigationRules (CurrentQuestionCode, AnswerValue, RecommendedBucket, ConditionType, Priority) VALUES ('Coverage', 'Only core systems', 'Lite', NULL, 2);
INSERT INTO NavigationRules (CurrentQuestionCode, AnswerValue, NextQuestionCode, ConditionType, Priority) VALUES ('Coverage', 'Most departments', 'EnvSelection', 'HasEnvironments', 1);
INSERT INTO NavigationRules (CurrentQuestionCode, AnswerValue, NextQuestionCode, ConditionType, Priority) VALUES ('Coverage', 'Most departments', 'Environments', NULL, 2);
INSERT INTO NavigationRules (CurrentQuestionCode, AnswerValue, NextQuestionCode, ConditionType, Priority) VALUES ('Coverage', 'Full organization', 'EnvSelection', 'HasEnvironments', 1);
INSERT INTO NavigationRules (CurrentQuestionCode, AnswerValue, NextQuestionCode, ConditionType, Priority) VALUES ('Coverage', 'Full organization', 'Environments', NULL, 2);

INSERT INTO NavigationRules (CurrentQuestionCode, AnswerValue, RecommendedBucket, ConditionType, Priority) VALUES ('Environments', '1', 'Lite', NULL, 1);
INSERT INTO NavigationRules (CurrentQuestionCode, AnswerValue, RecommendedBucket, ConditionType, Priority) VALUES ('Environments', '2-3', 'Medium', NULL, 1);
INSERT INTO NavigationRules (CurrentQuestionCode, AnswerValue, RecommendedBucket, ConditionType, Priority) VALUES ('Environments', '4-10', 'Premium', NULL, 1);
INSERT INTO NavigationRules (CurrentQuestionCode, AnswerValue, RecommendedBucket, ConditionType, Priority) VALUES ('Environments', 'More than 10', 'Premium', NULL, 1);
GO 

INSERT INTO Organizations (Name) VALUES ('Small Startup (No Env)');
INSERT INTO Organizations (Name) VALUES ('Medium Business');
INSERT INTO Organizations (Name) VALUES ('Large Corp (No Env)');

DECLARE @OrgId INT;

INSERT INTO Organizations (Name) VALUES ('Large Corp (With Env)');
SET @OrgId = SCOPE_IDENTITY();

INSERT INTO Environments (OrganizationId, Name) VALUES (@OrgId, 'Production');
INSERT INTO Environments (OrganizationId, Name) VALUES (@OrgId, 'Staging');
INSERT INTO Environments (OrganizationId, Name) VALUES (@OrgId, 'Development');
INSERT INTO Environments (OrganizationId, Name) VALUES (@OrgId, 'QA');
GO 
