use McWizardDB;
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