CREATE TABLE [dbo].[usersaccounts] (
    [Id]       INT          IDENTITY (1, 1) NOT NULL,
    [name]     VARCHAR (50) NULL,
    [pass]     VARCHAR (50) NULL,
    [role]     VARCHAR(50)  NULL, 
    PRIMARY KEY CLUSTERED ([Id] ASC)
);

CREATE TABLE [dbo].[book] (
    [Id]      INT             IDENTITY (1, 1) NOT NULL,
    [title]   VARCHAR (100)   NOT NULL,
    [price]   DECIMAL(10, 2)  NOT NULL,
    [imgfile] VARCHAR (100)   NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);

CREATE TABLE [dbo].[bookorder] (
    [Id]        INT          IDENTITY (1, 1) NOT NULL,
    [custname]  VARCHAR (50) NOT NULL,
    [orderdate] DATE         DEFAULT (getdate()) NOT NULL,
    [total]     INT          NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);

-- Insert sample users
INSERT INTO [dbo].[usersaccounts] ([name], [pass], [role]) VALUES
('admin', 'admin123', 'admin'),
('john', 'john123', 'customer'),
('jane', 'jane123', 'customer');
