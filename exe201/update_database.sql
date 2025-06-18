-- Create Size table if it doesn't exist
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Sizes')
BEGIN
    CREATE TABLE [Sizes] (
        [Id] int NOT NULL IDENTITY,
        [Name] nvarchar(max) NOT NULL,
        CONSTRAINT [PK_Sizes] PRIMARY KEY ([Id])
    );

    -- Add some default sizes
    INSERT INTO [Sizes] ([Name]) VALUES ('Nhỏ');
    INSERT INTO [Sizes] ([Name]) VALUES ('Trung Bình');
    INSERT INTO [Sizes] ([Name]) VALUES ('Lớn');
END

-- Add SizeId column to CartItems if it doesn't exist
IF NOT EXISTS (SELECT * FROM sys.columns WHERE name = 'SizeId' AND object_id = OBJECT_ID('CartItems'))
BEGIN
    ALTER TABLE [CartItems] ADD [SizeId] int NOT NULL DEFAULT 1;
    
    -- Add foreign key constraint
    ALTER TABLE [CartItems] ADD CONSTRAINT [FK_CartItems_Sizes_SizeId] 
    FOREIGN KEY ([SizeId]) REFERENCES [Sizes] ([Id]) ON DELETE CASCADE;
    
    -- Create index
    CREATE INDEX [IX_CartItems_SizeId] ON [CartItems] ([SizeId]);
END

-- Create OrderItems table if it doesn't exist
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'OrderItems')
BEGIN
    CREATE TABLE [OrderItems] (
        [Id] int NOT NULL IDENTITY,
        [OrderId] int NOT NULL,
        [ProductId] int NOT NULL,
        [SizeId] int NOT NULL,
        [Quantity] int NOT NULL,
        [Price] decimal(18,2) NOT NULL,
        CONSTRAINT [PK_OrderItems] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_OrderItems_Orders_OrderId] FOREIGN KEY ([OrderId]) REFERENCES [Orders] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_OrderItems_Products_ProductId] FOREIGN KEY ([ProductId]) REFERENCES [Products] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_OrderItems_Sizes_SizeId] FOREIGN KEY ([SizeId]) REFERENCES [Sizes] ([Id]) ON DELETE CASCADE
    );

    -- Create indexes
    CREATE INDEX [IX_OrderItems_OrderId] ON [OrderItems] ([OrderId]);
    CREATE INDEX [IX_OrderItems_ProductId] ON [OrderItems] ([ProductId]);
    CREATE INDEX [IX_OrderItems_SizeId] ON [OrderItems] ([SizeId]);
END 