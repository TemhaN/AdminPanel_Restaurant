﻿USE [master]
GO
/****** Object:  Database [restaurant]    Script Date: 13.02.2025 16:56:22 ******/
CREATE DATABASE [restaurant]
 CONTAINMENT = NONE
 ON  PRIMARY 
( NAME = N'restaurant', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL16.TDG2022\MSSQL\DATA\restaurant.mdf' , SIZE = 8192KB , MAXSIZE = UNLIMITED, FILEGROWTH = 65536KB )
 LOG ON 
( NAME = N'restaurant_log', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL16.TDG2022\MSSQL\DATA\restaurant_log.ldf' , SIZE = 8192KB , MAXSIZE = 2048GB , FILEGROWTH = 65536KB )
 WITH CATALOG_COLLATION = DATABASE_DEFAULT, LEDGER = OFF
GO
ALTER DATABASE [restaurant] SET COMPATIBILITY_LEVEL = 150
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [restaurant].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO
ALTER DATABASE [restaurant] SET ANSI_NULL_DEFAULT OFF 
GO
ALTER DATABASE [restaurant] SET ANSI_NULLS OFF 
GO
ALTER DATABASE [restaurant] SET ANSI_PADDING OFF 
GO
ALTER DATABASE [restaurant] SET ANSI_WARNINGS OFF 
GO
ALTER DATABASE [restaurant] SET ARITHABORT OFF 
GO
ALTER DATABASE [restaurant] SET AUTO_CLOSE ON 
GO
ALTER DATABASE [restaurant] SET AUTO_SHRINK OFF 
GO
ALTER DATABASE [restaurant] SET AUTO_UPDATE_STATISTICS ON 
GO
ALTER DATABASE [restaurant] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [restaurant] SET CURSOR_DEFAULT  GLOBAL 
GO
ALTER DATABASE [restaurant] SET CONCAT_NULL_YIELDS_NULL OFF 
GO
ALTER DATABASE [restaurant] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [restaurant] SET QUOTED_IDENTIFIER OFF 
GO
ALTER DATABASE [restaurant] SET RECURSIVE_TRIGGERS OFF 
GO
ALTER DATABASE [restaurant] SET  ENABLE_BROKER 
GO
ALTER DATABASE [restaurant] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [restaurant] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO
ALTER DATABASE [restaurant] SET TRUSTWORTHY OFF 
GO
ALTER DATABASE [restaurant] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO
ALTER DATABASE [restaurant] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [restaurant] SET READ_COMMITTED_SNAPSHOT OFF 
GO
ALTER DATABASE [restaurant] SET HONOR_BROKER_PRIORITY OFF 
GO
ALTER DATABASE [restaurant] SET RECOVERY SIMPLE 
GO
ALTER DATABASE [restaurant] SET  MULTI_USER 
GO
ALTER DATABASE [restaurant] SET PAGE_VERIFY CHECKSUM  
GO
ALTER DATABASE [restaurant] SET DB_CHAINING OFF 
GO
ALTER DATABASE [restaurant] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) 
GO
ALTER DATABASE [restaurant] SET TARGET_RECOVERY_TIME = 60 SECONDS 
GO
ALTER DATABASE [restaurant] SET DELAYED_DURABILITY = DISABLED 
GO
ALTER DATABASE [restaurant] SET ACCELERATED_DATABASE_RECOVERY = OFF  
GO
ALTER DATABASE [restaurant] SET QUERY_STORE = OFF
GO
USE [restaurant]
GO
/****** Object:  Table [dbo].[authors]    Script Date: 13.02.2025 16:56:22 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[authors](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[ФИО] [nvarchar](255) NULL,
	[дата_рождения] [date] NULL,
PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[categories]    Script Date: 13.02.2025 16:56:22 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[categories](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[Название] [nvarchar](255) NOT NULL,
	[dish_id] [int] NULL,
	[menu_id] [int] NULL,
PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[dishes]    Script Date: 13.02.2025 16:56:22 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[dishes](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[название] [nvarchar](255) NOT NULL,
	[дата_создания] [date] NOT NULL,
	[автор_id] [int] NULL,
PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[DishIngredient]    Script Date: 13.02.2025 16:56:22 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DishIngredient](
	[dish_id] [int] NOT NULL,
	[ingredient_id] [int] NOT NULL,
	[количество] [decimal](10, 2) NULL,
PRIMARY KEY CLUSTERED 
(
	[dish_id] ASC,
	[ingredient_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ingredients]    Script Date: 13.02.2025 16:56:22 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ingredients](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[название] [nvarchar](255) NOT NULL,
	[количество] [decimal](10, 2) NULL,
	[единица_измерения] [nvarchar](50) NULL,
	[калории_на_100г] [decimal](5, 2) NULL,
	[белки_на_100г] [decimal](5, 2) NULL,
	[жиры_на_100г] [decimal](5, 2) NULL,
	[углеводы_на_100г] [decimal](5, 2) NULL,
PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[menu]    Script Date: 13.02.2025 16:56:22 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[menu](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[Название] [nvarchar](255) NOT NULL,
	[Дата] [date] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[orders]    Script Date: 13.02.2025 16:56:22 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[orders](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[Дата] [date] NOT NULL,
	[Количество] [int] NOT NULL,
	[dish_id] [int] NULL,
	[client_name] [nvarchar](100) NULL,
PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[categories]  WITH CHECK ADD FOREIGN KEY([dish_id])
REFERENCES [dbo].[dishes] ([id])
GO
ALTER TABLE [dbo].[categories]  WITH CHECK ADD FOREIGN KEY([dish_id])
REFERENCES [dbo].[dishes] ([id])
GO
ALTER TABLE [dbo].[categories]  WITH CHECK ADD FOREIGN KEY([menu_id])
REFERENCES [dbo].[menu] ([id])
GO
ALTER TABLE [dbo].[categories]  WITH CHECK ADD FOREIGN KEY([menu_id])
REFERENCES [dbo].[menu] ([id])
GO
ALTER TABLE [dbo].[dishes]  WITH CHECK ADD FOREIGN KEY([автор_id])
REFERENCES [dbo].[authors] ([id])
GO
ALTER TABLE [dbo].[dishes]  WITH CHECK ADD FOREIGN KEY([автор_id])
REFERENCES [dbo].[authors] ([id])
GO
ALTER TABLE [dbo].[DishIngredient]  WITH CHECK ADD FOREIGN KEY([dish_id])
REFERENCES [dbo].[dishes] ([id])
GO
ALTER TABLE [dbo].[DishIngredient]  WITH CHECK ADD FOREIGN KEY([dish_id])
REFERENCES [dbo].[dishes] ([id])
GO
ALTER TABLE [dbo].[DishIngredient]  WITH CHECK ADD FOREIGN KEY([ingredient_id])
REFERENCES [dbo].[ingredients] ([id])
GO
ALTER TABLE [dbo].[DishIngredient]  WITH CHECK ADD FOREIGN KEY([ingredient_id])
REFERENCES [dbo].[ingredients] ([id])
GO
ALTER TABLE [dbo].[orders]  WITH CHECK ADD  CONSTRAINT [FK_orders_dish_id] FOREIGN KEY([dish_id])
REFERENCES [dbo].[dishes] ([id])
GO
ALTER TABLE [dbo].[orders] CHECK CONSTRAINT [FK_orders_dish_id]
GO
USE [master]
GO
ALTER DATABASE [restaurant] SET  READ_WRITE 
GO
