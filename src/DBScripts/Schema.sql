CREATE DATABASE McWizardDB;
GO

USE McWizardDB;
GO

CREATE TABLE McWizardDB.Organizations (
    Id INT PRIMARY KEY IDENTITY,
    Name NVARCHAR(255) NOT NULL,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE()
);

CREATE TABLE McWizardDB.Environments (
    Id INT PRIMARY KEY IDENTITY,
    OrganizationId INT NOT NULL,
    Name NVARCHAR(255) NOT NULL,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    FOREIGN KEY (OrganizationId) REFERENCES Organizations(Id)
);

CREATE TABLE McWizardDB.Questions (
    Code NVARCHAR(50) PRIMARY KEY,
    Text NVARCHAR(MAX) NOT NULL,
    Type NVARCHAR(50) NOT NULL
);

CREATE TABLE McWizardDB.QuestionOptions (
    Id INT PRIMARY KEY IDENTITY,
    QuestionCode NVARCHAR(50) NOT NULL,
    Value NVARCHAR(255) NOT NULL,
    DisplayText NVARCHAR(255) NOT NULL,
    SortOrder INT NOT NULL DEFAULT 0,
    FOREIGN KEY (QuestionCode) REFERENCES Questions(Code)
);

CREATE TABLE McWizardDB.BucketConfigurations (
    BucketName NVARCHAR(50) PRIMARY KEY,
    MinEnvironments INT NOT NULL,
    MaxEnvironments INT
);

CREATE TABLE McWizardDB.NavigationRules (
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

CREATE TABLE McWizardDB.WizardSessions (
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

CREATE TABLE McWizardDB.SessionAnswers (
    Id INT PRIMARY KEY IDENTITY,
    SessionId UNIQUEIDENTIFIER NOT NULL,
    QuestionCode NVARCHAR(50) NOT NULL,
    Answer NVARCHAR(MAX) NOT NULL,
    AnsweredAt DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    FOREIGN KEY (SessionId) REFERENCES WizardSessions(Id)
);
GO 