-- ----------------------------------------------------------------------------
-- MySQL Workbench Migration
-- Migrated Schemata: ro_ticketing
-- Source Schemata: ro_ticketing
-- Created: Thu Aug 30 00:45:57 2018
-- Workbench Version: 8.0.12
-- ----------------------------------------------------------------------------

SET FOREIGN_KEY_CHECKS = 0;

-- ----------------------------------------------------------------------------
-- Schema ro_ticketing
-- ----------------------------------------------------------------------------
DROP SCHEMA IF EXISTS `ro_ticketing` ;
CREATE SCHEMA IF NOT EXISTS `ro_ticketing` ;

-- ----------------------------------------------------------------------------
-- Table ro_ticketing.appuser
-- ----------------------------------------------------------------------------
CREATE TABLE IF NOT EXISTS `ro_ticketing`.`appuser` (
  `ID` INT(11) NOT NULL AUTO_INCREMENT,
  `AspNetUserId` TEXT NULL DEFAULT NULL,
  `ClientId` INT(11) NOT NULL,
  `EffectiveDate` DATETIME NOT NULL,
  `Email` TINYTEXT NOT NULL,
  `FirstName` TINYTEXT NOT NULL,
  `LastName` TINYTEXT NOT NULL,
  `OrgCode` TINYTEXT NOT NULL,
  `RepId` TINYTEXT NOT NULL,
  `Role` TINYTEXT NOT NULL,
  `SetPasswordToken` TEXT NULL DEFAULT NULL,
  `CreatedAt` DATETIME NOT NULL DEFAULT '0001-01-01 00:00:00',
  `CreatedBy` TINYTEXT NULL DEFAULT NULL,
  `UpdatedAt` DATETIME NOT NULL DEFAULT '0001-01-01 00:00:00',
  `UpdatedBy` TINYTEXT NULL DEFAULT NULL,
  `IsDeleted` BIT(1) NOT NULL,
  `IsEnabled` BIT(1) NOT NULL,
  `UserGuid` CHAR(36) NULL DEFAULT NULL,
  PRIMARY KEY (`ID`))
ENGINE = InnoDB
AUTO_INCREMENT = 9
DEFAULT CHARACTER SET = utf8;

-- ----------------------------------------------------------------------------
-- Table ro_ticketing.aspnetroleclaims
-- ----------------------------------------------------------------------------
CREATE TABLE IF NOT EXISTS `ro_ticketing`.`aspnetroleclaims` (
  `Id` INT(11) NOT NULL AUTO_INCREMENT,
  `ClaimType` TEXT NULL DEFAULT NULL,
  `ClaimValue` TEXT NULL DEFAULT NULL,
  `RoleId` VARCHAR(128) NOT NULL,
  PRIMARY KEY (`Id`),
  INDEX `IX_AspNetRoleClaims_RoleId` (`RoleId` ASC),
  CONSTRAINT `FK_AspNetRoleClaims_AspNetRoles_RoleId`
    FOREIGN KEY (`RoleId`)
    REFERENCES `ro_ticketing`.`aspnetroles` (`Id`)
    ON DELETE CASCADE)
ENGINE = InnoDB
DEFAULT CHARACTER SET = utf8mb4
COLLATE = utf8mb4_0900_ai_ci;

-- ----------------------------------------------------------------------------
-- Table ro_ticketing.aspnetroles
-- ----------------------------------------------------------------------------
CREATE TABLE IF NOT EXISTS `ro_ticketing`.`aspnetroles` (
  `Id` VARCHAR(128) NOT NULL,
  `ConcurrencyStamp` TEXT NULL DEFAULT NULL,
  `Name` VARCHAR(128) NULL DEFAULT NULL,
  `NormalizedName` VARCHAR(256) NULL DEFAULT NULL,
  PRIMARY KEY (`Id`),
  UNIQUE INDEX `RoleNameIndex` (`NormalizedName`(255) ASC))
ENGINE = InnoDB
DEFAULT CHARACTER SET = utf8mb4
COLLATE = utf8mb4_0900_ai_ci;

-- ----------------------------------------------------------------------------
-- Table ro_ticketing.aspnetuserclaims
-- ----------------------------------------------------------------------------
CREATE TABLE IF NOT EXISTS `ro_ticketing`.`aspnetuserclaims` (
  `Id` INT(11) NOT NULL AUTO_INCREMENT,
  `ClaimType` TEXT NULL DEFAULT NULL,
  `ClaimValue` TEXT NULL DEFAULT NULL,
  `UserId` VARCHAR(128) NOT NULL,
  PRIMARY KEY (`Id`),
  INDEX `IX_AspNetUserClaims_UserId` (`UserId` ASC),
  CONSTRAINT `FK_AspNetUserClaims_AspNetUsers_UserId`
    FOREIGN KEY (`UserId`)
    REFERENCES `ro_ticketing`.`aspnetusers` (`Id`)
    ON DELETE CASCADE)
ENGINE = InnoDB
DEFAULT CHARACTER SET = utf8mb4
COLLATE = utf8mb4_0900_ai_ci;

-- ----------------------------------------------------------------------------
-- Table ro_ticketing.aspnetuserlogins
-- ----------------------------------------------------------------------------
CREATE TABLE IF NOT EXISTS `ro_ticketing`.`aspnetuserlogins` (
  `LoginProvider` VARCHAR(127) NOT NULL,
  `ProviderKey` VARCHAR(127) NOT NULL,
  `ProviderDisplayName` TEXT NULL DEFAULT NULL,
  `UserId` VARCHAR(128) NOT NULL,
  PRIMARY KEY (`LoginProvider`, `ProviderKey`),
  INDEX `IX_AspNetUserLogins_UserId` (`UserId` ASC),
  CONSTRAINT `FK_AspNetUserLogins_AspNetUsers_UserId`
    FOREIGN KEY (`UserId`)
    REFERENCES `ro_ticketing`.`aspnetusers` (`Id`)
    ON DELETE CASCADE)
ENGINE = InnoDB
DEFAULT CHARACTER SET = utf8mb4
COLLATE = utf8mb4_0900_ai_ci;

-- ----------------------------------------------------------------------------
-- Table ro_ticketing.aspnetuserroles
-- ----------------------------------------------------------------------------
CREATE TABLE IF NOT EXISTS `ro_ticketing`.`aspnetuserroles` (
  `UserId` VARCHAR(128) NOT NULL,
  `RoleId` VARCHAR(128) NOT NULL,
  PRIMARY KEY (`UserId`, `RoleId`),
  INDEX `IX_AspNetUserRoles_RoleId` (`RoleId` ASC),
  CONSTRAINT `FK_AspNetUserRoles_AspNetRoles_RoleId`
    FOREIGN KEY (`RoleId`)
    REFERENCES `ro_ticketing`.`aspnetroles` (`Id`)
    ON DELETE CASCADE,
  CONSTRAINT `FK_AspNetUserRoles_AspNetUsers_UserId`
    FOREIGN KEY (`UserId`)
    REFERENCES `ro_ticketing`.`aspnetusers` (`Id`)
    ON DELETE CASCADE)
ENGINE = InnoDB
DEFAULT CHARACTER SET = utf8mb4
COLLATE = utf8mb4_0900_ai_ci;

-- ----------------------------------------------------------------------------
-- Table ro_ticketing.aspnetusers
-- ----------------------------------------------------------------------------
CREATE TABLE IF NOT EXISTS `ro_ticketing`.`aspnetusers` (
  `Id` VARCHAR(128) NOT NULL,
  `AccessFailedCount` INT(11) NOT NULL,
  `ConcurrencyStamp` TEXT NULL DEFAULT NULL,
  `Email` VARCHAR(256) NULL DEFAULT NULL,
  `EmailConfirmed` BIT(1) NOT NULL,
  `LockoutEnabled` BIT(1) NOT NULL,
  `LockoutEnd` TIMESTAMP NULL DEFAULT NULL,
  `NormalizedEmail` VARCHAR(256) NULL DEFAULT NULL,
  `NormalizedUserName` VARCHAR(256) NULL DEFAULT NULL,
  `PasswordHash` TEXT NULL DEFAULT NULL,
  `PhoneNumber` TEXT NULL DEFAULT NULL,
  `PhoneNumberConfirmed` BIT(1) NOT NULL,
  `SecurityStamp` TEXT NULL DEFAULT NULL,
  `TwoFactorEnabled` BIT(1) NOT NULL,
  `UserName` VARCHAR(128) NULL DEFAULT NULL,
  PRIMARY KEY (`Id`),
  UNIQUE INDEX `UserNameIndex` (`NormalizedUserName`(255) ASC),
  INDEX `EmailIndex` (`NormalizedEmail`(255) ASC))
ENGINE = InnoDB
DEFAULT CHARACTER SET = utf8mb4
COLLATE = utf8mb4_0900_ai_ci;

-- ----------------------------------------------------------------------------
-- Table ro_ticketing.aspnetusertokens
-- ----------------------------------------------------------------------------
CREATE TABLE IF NOT EXISTS `ro_ticketing`.`aspnetusertokens` (
  `UserId` VARCHAR(128) NOT NULL,
  `LoginProvider` VARCHAR(128) NOT NULL,
  `Name` VARCHAR(128) NOT NULL,
  `Value` TEXT NULL DEFAULT NULL,
  PRIMARY KEY (`UserId`, `LoginProvider`, `Name`),
  CONSTRAINT `FK_AspNetUserTokens_AspNetUsers_UserId`
    FOREIGN KEY (`UserId`)
    REFERENCES `ro_ticketing`.`aspnetusers` (`Id`)
    ON DELETE CASCADE)
ENGINE = InnoDB
DEFAULT CHARACTER SET = utf8mb4
COLLATE = utf8mb4_0900_ai_ci;

-- ----------------------------------------------------------------------------
-- Table ro_ticketing.audit
-- ----------------------------------------------------------------------------
CREATE TABLE IF NOT EXISTS `ro_ticketing`.`audit` (
  `Id` INT(11) NOT NULL AUTO_INCREMENT,
  `ChangedBy` TEXT NULL DEFAULT NULL,
  `DateTime` DATETIME NOT NULL,
  `KeyValues` TEXT NULL DEFAULT NULL,
  `NewValues` TEXT NULL DEFAULT NULL,
  `OldValues` TEXT NULL DEFAULT NULL,
  `TableName` TEXT NULL DEFAULT NULL,
  PRIMARY KEY (`Id`))
ENGINE = InnoDB
AUTO_INCREMENT = 8360
DEFAULT CHARACTER SET = utf8;

-- ----------------------------------------------------------------------------
-- Table ro_ticketing.customer
-- ----------------------------------------------------------------------------
CREATE TABLE IF NOT EXISTS `ro_ticketing`.`customer` (
  `Id` INT(11) NOT NULL AUTO_INCREMENT,
  `Name` VARCHAR(255) NOT NULL,
  `FuelSurchargeFee` FLOAT NOT NULL DEFAULT '0',
  `IsEnabled` BIT(1) NOT NULL DEFAULT b'1',
  `IsDeleted` BIT(1) NOT NULL DEFAULT b'0',
  `CreatedAt` DATETIME NOT NULL DEFAULT '0001-01-01 00:00:00',
  `CreatedBy` TINYTEXT NULL DEFAULT NULL,
  `UpdatedAt` DATETIME NOT NULL DEFAULT '0001-01-01 00:00:00',
  `UpdatedBy` TINYTEXT NULL DEFAULT NULL,
  PRIMARY KEY (`Id`))
ENGINE = InnoDB
AUTO_INCREMENT = 146
DEFAULT CHARACTER SET = utf8;

-- ----------------------------------------------------------------------------
-- Table ro_ticketing.driver
-- ----------------------------------------------------------------------------
CREATE TABLE IF NOT EXISTS `ro_ticketing`.`driver` (
  `Id` INT(11) NOT NULL AUTO_INCREMENT,
  `Name` VARCHAR(255) NOT NULL,
  `Code` VARCHAR(255) NOT NULL,
  `Phone` VARCHAR(255) NOT NULL,
  `Position` VARCHAR(255) NOT NULL,
  `IsEnabled` BIT(1) NOT NULL,
  `IsDeleted` BIT(1) NOT NULL,
  `CreatedAt` DATETIME NOT NULL DEFAULT '0001-01-01 00:00:00',
  `CreatedBy` TINYTEXT NULL DEFAULT NULL,
  `UpdatedAt` DATETIME NOT NULL DEFAULT '0001-01-01 00:00:00',
  `UpdatedBy` TINYTEXT NULL DEFAULT NULL,
  PRIMARY KEY (`Id`))
ENGINE = InnoDB
AUTO_INCREMENT = 96
DEFAULT CHARACTER SET = utf8;

-- ----------------------------------------------------------------------------
-- Table ro_ticketing.jobtype
-- ----------------------------------------------------------------------------
CREATE TABLE IF NOT EXISTS `ro_ticketing`.`jobtype` (
  `Id` INT(11) NOT NULL AUTO_INCREMENT,
  `Name` VARCHAR(255) NOT NULL,
  `IsEnabled` BIT(1) NOT NULL,
  `IsDeleted` BIT(1) NOT NULL,
  `CreatedAt` DATETIME NOT NULL DEFAULT '0001-01-01 00:00:00',
  `CreatedBy` TINYTEXT NULL DEFAULT NULL,
  `UpdatedAt` DATETIME NOT NULL DEFAULT '0001-01-01 00:00:00',
  `UpdatedBy` TINYTEXT NULL DEFAULT NULL,
  PRIMARY KEY (`Id`))
ENGINE = InnoDB
AUTO_INCREMENT = 17
DEFAULT CHARACTER SET = utf8;

-- ----------------------------------------------------------------------------
-- Table ro_ticketing.market
-- ----------------------------------------------------------------------------
CREATE TABLE IF NOT EXISTS `ro_ticketing`.`market` (
  `id` INT(11) UNSIGNED NOT NULL AUTO_INCREMENT,
  `code` VARCHAR(10) NOT NULL,
  `description` VARCHAR(100) NOT NULL,
  `CreatedAt` DATETIME NOT NULL DEFAULT '0001-01-01 00:00:00',
  `CreatedBy` TINYTEXT NULL DEFAULT NULL,
  `UpdatedAt` DATETIME NOT NULL DEFAULT '0001-01-01 00:00:00',
  `UpdatedBy` TINYTEXT NULL DEFAULT NULL,
  `IsEnabled` BIT(1) NOT NULL DEFAULT b'1',
  `IsDeleted` BIT(1) NOT NULL DEFAULT b'0',
  PRIMARY KEY (`id`))
ENGINE = InnoDB
AUTO_INCREMENT = 20
DEFAULT CHARACTER SET = utf8;

-- ----------------------------------------------------------------------------
-- Table ro_ticketing.order
-- ----------------------------------------------------------------------------
CREATE TABLE IF NOT EXISTS `ro_ticketing`.`order` (
  `Id` INT(11) NOT NULL AUTO_INCREMENT,
  `OrderDate` DATETIME(6) NOT NULL,
  `RequestDate` DATETIME NOT NULL,
  `RequestTime` VARCHAR(15) NOT NULL,
  `OrderStatusId` TINYINT(3) UNSIGNED NOT NULL,
  `JobTypeId` SMALLINT(6) NOT NULL,
  `CustomerId` INT(11) NOT NULL,
  `RigLocationId` INT(11) NOT NULL,
  `WellId` INT(11) NOT NULL,
  `AFE_PO` VARCHAR(50) NULL DEFAULT NULL,
  `PointOfContactName` VARCHAR(255) NULL DEFAULT NULL,
  `PointOfContactNumber` VARCHAR(255) NULL DEFAULT NULL,
  `LoadOrigin` INT(11) NOT NULL,
  `OrderDescription` VARCHAR(1024) NOT NULL,
  `IsEnabled` BIT(1) NOT NULL,
  `IsDeleted` BIT(1) NOT NULL,
  `CreatedAt` DATETIME NOT NULL DEFAULT '0001-01-01 00:00:00',
  `CreatedBy` TINYTEXT NULL DEFAULT NULL,
  `UpdatedAt` DATETIME NOT NULL DEFAULT '0001-01-01 00:00:00',
  `UpdatedBy` TINYTEXT NULL DEFAULT NULL,
  PRIMARY KEY (`Id`))
ENGINE = InnoDB
AUTO_INCREMENT = 158
DEFAULT CHARACTER SET = utf8;

-- ----------------------------------------------------------------------------
-- Table ro_ticketing.product
-- ----------------------------------------------------------------------------
CREATE TABLE IF NOT EXISTS `ro_ticketing`.`product` (
  `Id` INT(11) NOT NULL AUTO_INCREMENT,
  `Name` VARCHAR(255) NOT NULL,
  `Unit` TINYTEXT NULL DEFAULT NULL,
  `IsIncludedInReport` BIT(1) NOT NULL DEFAULT b'1',
  `IsEnabled` BIT(1) NOT NULL DEFAULT b'1',
  `IsDeleted` BIT(1) NOT NULL DEFAULT b'0',
  `CreatedAt` DATETIME NOT NULL DEFAULT '0001-01-01 00:00:00',
  `CreatedBy` TINYTEXT NULL DEFAULT NULL,
  `UpdatedAt` DATETIME NOT NULL DEFAULT '0001-01-01 00:00:00',
  `UpdatedBy` TINYTEXT NULL DEFAULT NULL,
  PRIMARY KEY (`Id`))
ENGINE = InnoDB
DEFAULT CHARACTER SET = utf8;

-- ----------------------------------------------------------------------------
-- Table ro_ticketing.riglocation
-- ----------------------------------------------------------------------------
CREATE TABLE IF NOT EXISTS `ro_ticketing`.`riglocation` (
  `Id` INT(11) NOT NULL AUTO_INCREMENT,
  `CustomerId` INT(11) NOT NULL,
  `Name` VARCHAR(255) NOT NULL,
  `IsEnabled` BIT(1) NOT NULL,
  `IsDeleted` BIT(1) NOT NULL,
  `CreatedAt` DATETIME NOT NULL DEFAULT '0001-01-01 00:00:00',
  `CreatedBy` TINYTEXT NULL DEFAULT NULL,
  `UpdatedAt` DATETIME NOT NULL DEFAULT '0001-01-01 00:00:00',
  `UpdatedBy` TINYTEXT NULL DEFAULT NULL,
  PRIMARY KEY (`Id`))
ENGINE = InnoDB
AUTO_INCREMENT = 10
DEFAULT CHARACTER SET = utf8;

-- ----------------------------------------------------------------------------
-- Table ro_ticketing.setting
-- ----------------------------------------------------------------------------
CREATE TABLE IF NOT EXISTS `ro_ticketing`.`setting` (
  `Id` INT(11) NOT NULL AUTO_INCREMENT,
  `Key` VARCHAR(255) NOT NULL,
  `Value` VARCHAR(255) NOT NULL,
  `IsEnabled` BIT(1) NOT NULL,
  `IsDeleted` BIT(1) NOT NULL,
  `CreatedAt` DATETIME NOT NULL DEFAULT '0001-01-01 00:00:00',
  `CreatedBy` TINYTEXT NULL DEFAULT NULL,
  `UpdatedAt` DATETIME NOT NULL DEFAULT '0001-01-01 00:00:00',
  `UpdatedBy` TINYTEXT NULL DEFAULT NULL,
  PRIMARY KEY (`Id`))
ENGINE = InnoDB
AUTO_INCREMENT = 9
DEFAULT CHARACTER SET = utf8;

-- ----------------------------------------------------------------------------
-- Table ro_ticketing.well
-- ----------------------------------------------------------------------------
CREATE TABLE IF NOT EXISTS `ro_ticketing`.`well` (
  `Id` INT(11) NOT NULL AUTO_INCREMENT,
  `Name` VARCHAR(255) NOT NULL,
  `IsEnabled` BIT(1) NOT NULL,
  `IsDeleted` BIT(1) NOT NULL,
  `CreatedAt` DATETIME NOT NULL DEFAULT '0001-01-01 00:00:00',
  `CreatedBy` TINYTEXT NULL DEFAULT NULL,
  `UpdatedAt` DATETIME NOT NULL DEFAULT '0001-01-01 00:00:00',
  `UpdatedBy` TINYTEXT NULL DEFAULT NULL,
  `RigLocationId` INT(11) NOT NULL,
  PRIMARY KEY (`Id`))
ENGINE = InnoDB
AUTO_INCREMENT = 12
DEFAULT CHARACTER SET = utf8;
SET FOREIGN_KEY_CHECKS = 1;
