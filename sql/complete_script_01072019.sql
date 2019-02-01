-- MySQL Workbench Forward Engineering

SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0;
SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0;
SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='TRADITIONAL,ALLOW_INVALID_DATES';

-- -----------------------------------------------------
-- Schema mydb
-- -----------------------------------------------------
-- -----------------------------------------------------
-- Schema ro_ticketing
-- -----------------------------------------------------

-- -----------------------------------------------------
-- Schema ro_ticketing
-- -----------------------------------------------------
CREATE SCHEMA IF NOT EXISTS `ro_ticketing` DEFAULT CHARACTER SET latin1 ;
USE `ro_ticketing` ;

-- -----------------------------------------------------
-- Table `ro_ticketing`.`appuser`
-- -----------------------------------------------------
DROP TABLE IF EXISTS `ro_ticketing`.`appuser` ;

CREATE TABLE IF NOT EXISTS `ro_ticketing`.`appuser` (
  `ID` INT(11) NOT NULL AUTO_INCREMENT,
  `AspNetUserId` TEXT CHARACTER SET 'utf8' COLLATE 'utf8_bin' NULL DEFAULT NULL,
  `ClientId` INT(11) NOT NULL,
  `EffectiveDate` DATETIME NOT NULL,
  `FirstName` TINYTEXT CHARACTER SET 'utf8' COLLATE 'utf8_bin' NOT NULL,
  `MiddleName` TINYTEXT CHARACTER SET 'utf8' COLLATE 'utf8_bin' NOT NULL,
  `LastName` TINYTEXT CHARACTER SET 'utf8' COLLATE 'utf8_bin' NOT NULL,
  `PhoneNumber` VARCHAR(25) CHARACTER SET 'utf8' COLLATE 'utf8_bin' NULL DEFAULT NULL,
  `Position` VARCHAR(30) CHARACTER SET 'utf8' COLLATE 'utf8_bin' NULL DEFAULT NULL,
  `EmployeeCode` VARCHAR(255) NOT NULL,
  `Email` TINYTEXT CHARACTER SET 'utf8' COLLATE 'utf8_bin' NOT NULL,
  `Username` VARCHAR(255) NULL DEFAULT NULL,
  `Role` TINYTEXT CHARACTER SET 'utf8' COLLATE 'utf8_bin' NOT NULL,
  `IsVisible` BIT(1) NOT NULL,
  `SetPasswordToken` TEXT CHARACTER SET 'utf8' COLLATE 'utf8_bin' NULL DEFAULT NULL,
  `CreatedAt` DATETIME NOT NULL DEFAULT '0001-01-01 00:00:00',
  `CreatedBy` TINYTEXT CHARACTER SET 'utf8' COLLATE 'utf8_bin' NULL DEFAULT NULL,
  `UpdatedAt` DATETIME NOT NULL DEFAULT '0001-01-01 00:00:00',
  `UpdatedBy` TINYTEXT CHARACTER SET 'utf8' COLLATE 'utf8_bin' NULL DEFAULT NULL,
  `IsDeleted` BIT(1) NOT NULL,
  `IsEnabled` BIT(1) NOT NULL,
  `UserGuid` CHAR(36) CHARACTER SET 'utf8' COLLATE 'utf8_bin' NULL DEFAULT NULL,
  PRIMARY KEY USING BTREE (`ID`))
ENGINE = InnoDB
AUTO_INCREMENT = 20
DEFAULT CHARACTER SET = utf8
ROW_FORMAT = DYNAMIC;


-- -----------------------------------------------------
-- Table `ro_ticketing`.`appuserlocation`
-- -----------------------------------------------------
DROP TABLE IF EXISTS `ro_ticketing`.`appuserlocation` ;

CREATE TABLE IF NOT EXISTS `ro_ticketing`.`appuserlocation` (
  `Id` INT(11) NOT NULL AUTO_INCREMENT,
  `AppUserId` VARCHAR(128) NOT NULL,
  `Latitude` FLOAT NOT NULL,
  `Longitude` FLOAT NOT NULL,
  `IsEnabled` BIT(1) NOT NULL DEFAULT b'1',
  `IsDeleted` BIT(1) NOT NULL DEFAULT b'0',
  `CreatedAt` DATETIME NOT NULL DEFAULT '0001-01-01 00:00:00',
  `CreatedBy` TINYTEXT NULL DEFAULT NULL,
  `UpdatedAt` DATETIME NOT NULL DEFAULT '0001-01-01 00:00:00',
  `UpdatedBy` TINYTEXT NULL DEFAULT NULL,
  PRIMARY KEY USING BTREE (`Id`))
ENGINE = InnoDB
DEFAULT CHARACTER SET = utf8
COLLATE = utf8_bin
ROW_FORMAT = DYNAMIC;


-- -----------------------------------------------------
-- Table `ro_ticketing`.`aspnetroles`
-- -----------------------------------------------------
DROP TABLE IF EXISTS `ro_ticketing`.`aspnetroles` ;

CREATE TABLE IF NOT EXISTS `ro_ticketing`.`aspnetroles` (
  `Id` VARCHAR(128) NOT NULL,
  `ConcurrencyStamp` TEXT NULL DEFAULT NULL,
  `Name` VARCHAR(128) NULL DEFAULT NULL,
  `NormalizedName` VARCHAR(256) NULL DEFAULT NULL,
  PRIMARY KEY (`Id`),
  UNIQUE INDEX `RoleNameIndex` (`NormalizedName` ASC))
ENGINE = InnoDB
DEFAULT CHARACTER SET = utf8
COLLATE = utf8_bin;


-- -----------------------------------------------------
-- Table `ro_ticketing`.`aspnetroleclaims`
-- -----------------------------------------------------
DROP TABLE IF EXISTS `ro_ticketing`.`aspnetroleclaims` ;

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
DEFAULT CHARACTER SET = utf8
COLLATE = utf8_bin;


-- -----------------------------------------------------
-- Table `ro_ticketing`.`aspnetusers`
-- -----------------------------------------------------
DROP TABLE IF EXISTS `ro_ticketing`.`aspnetusers` ;

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
  UNIQUE INDEX `UserNameIndex` (`NormalizedUserName` ASC),
  INDEX `EmailIndex` (`NormalizedEmail` ASC))
ENGINE = InnoDB
DEFAULT CHARACTER SET = utf8
COLLATE = utf8_bin;


-- -----------------------------------------------------
-- Table `ro_ticketing`.`aspnetuserclaims`
-- -----------------------------------------------------
DROP TABLE IF EXISTS `ro_ticketing`.`aspnetuserclaims` ;

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
DEFAULT CHARACTER SET = utf8
COLLATE = utf8_bin;


-- -----------------------------------------------------
-- Table `ro_ticketing`.`aspnetuserlogins`
-- -----------------------------------------------------
DROP TABLE IF EXISTS `ro_ticketing`.`aspnetuserlogins` ;

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
DEFAULT CHARACTER SET = utf8
COLLATE = utf8_bin;


-- -----------------------------------------------------
-- Table `ro_ticketing`.`aspnetuserroles`
-- -----------------------------------------------------
DROP TABLE IF EXISTS `ro_ticketing`.`aspnetuserroles` ;

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
DEFAULT CHARACTER SET = utf8
COLLATE = utf8_bin;


-- -----------------------------------------------------
-- Table `ro_ticketing`.`aspnetusertokens`
-- -----------------------------------------------------
DROP TABLE IF EXISTS `ro_ticketing`.`aspnetusertokens` ;

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
DEFAULT CHARACTER SET = utf8
COLLATE = utf8_bin;


-- -----------------------------------------------------
-- Table `ro_ticketing`.`audit`
-- -----------------------------------------------------
DROP TABLE IF EXISTS `ro_ticketing`.`audit` ;

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
AUTO_INCREMENT = 19985
DEFAULT CHARACTER SET = utf8;


-- -----------------------------------------------------
-- Table `ro_ticketing`.`categorytax`
-- -----------------------------------------------------
DROP TABLE IF EXISTS `ro_ticketing`.`categorytax` ;

CREATE TABLE IF NOT EXISTS `ro_ticketing`.`categorytax` (
  `Id` INT(11) NOT NULL AUTO_INCREMENT,
  `CategoryId` INT(11) NOT NULL,
  `TaxId` INT(11) NOT NULL,
  `IsEnabled` BIT(1) NOT NULL,
  `IsDeleted` BIT(1) NOT NULL,
  `CreatedAt` DATETIME NULL DEFAULT '0001-01-01 00:00:00',
  `CreatedBy` TINYTEXT NULL DEFAULT NULL,
  `UpdatedAt` DATETIME NULL DEFAULT '0001-01-01 00:00:00',
  `UpdatedBy` TINYTEXT NULL DEFAULT NULL,
  PRIMARY KEY USING BTREE (`Id`))
ENGINE = InnoDB
AUTO_INCREMENT = 9
DEFAULT CHARACTER SET = utf8
COLLATE = utf8_bin
ROW_FORMAT = DYNAMIC;


-- -----------------------------------------------------
-- Table `ro_ticketing`.`customer`
-- -----------------------------------------------------
DROP TABLE IF EXISTS `ro_ticketing`.`customer` ;

CREATE TABLE IF NOT EXISTS `ro_ticketing`.`customer` (
  `Id` INT(11) NOT NULL AUTO_INCREMENT,
  `Name` VARCHAR(255) NOT NULL,
  `ContactName` VARCHAR(255) NULL DEFAULT NULL,
  `ContactPhone` VARCHAR(25) NULL DEFAULT NULL,
  `Email` VARCHAR(100) NULL DEFAULT NULL,
  `FaxNumber` VARCHAR(25) NULL DEFAULT NULL,
  `BillToAddress` VARCHAR(255) NULL DEFAULT NULL,
  `ShipToAddress` VARCHAR(255) NULL DEFAULT NULL,
  `FuelSurchargeFee` DECIMAL(13,4) NOT NULL DEFAULT '0.0000',
  `PaymentTermId` INT(11) NOT NULL DEFAULT '1',
  `Note` VARCHAR(1500) NULL DEFAULT NULL,
  `IsVisible` BIT(1) NOT NULL DEFAULT b'1',
  `IsEnabled` BIT(1) NOT NULL DEFAULT b'1',
  `IsDeleted` BIT(1) NOT NULL DEFAULT b'0',
  `CreatedAt` DATETIME NOT NULL DEFAULT '0001-01-01 00:00:00',
  `CreatedBy` TINYTEXT NULL DEFAULT NULL,
  `UpdatedAt` DATETIME NOT NULL DEFAULT '0001-01-01 00:00:00',
  `UpdatedBy` TINYTEXT NULL DEFAULT NULL,
  PRIMARY KEY (`Id`))
ENGINE = InnoDB
AUTO_INCREMENT = 165
DEFAULT CHARACTER SET = utf8;


-- -----------------------------------------------------
-- Table `ro_ticketing`.`customernote`
-- -----------------------------------------------------
DROP TABLE IF EXISTS `ro_ticketing`.`customernote` ;

CREATE TABLE IF NOT EXISTS `ro_ticketing`.`customernote` (
  `Id` INT(11) NOT NULL AUTO_INCREMENT,
  `CustomerId` INT(11) NOT NULL,
  `CustomerNotes` VARCHAR(1025) NOT NULL,
  `IsEnabled` BIT(1) NOT NULL,
  `IsDeleted` BIT(1) NOT NULL,
  `CreatedAt` DATETIME NOT NULL DEFAULT '0001-01-01 00:00:00',
  `CreatedBy` TINYTEXT NULL DEFAULT NULL,
  `UpdatedAt` DATETIME NOT NULL DEFAULT '0001-01-01 00:00:00',
  `UpdatedBy` TINYTEXT NULL DEFAULT NULL,
  PRIMARY KEY USING BTREE (`Id`))
ENGINE = InnoDB
AUTO_INCREMENT = 6
DEFAULT CHARACTER SET = utf8
COLLATE = utf8_bin
ROW_FORMAT = DYNAMIC;


-- -----------------------------------------------------
-- Table `ro_ticketing`.`driver`
-- -----------------------------------------------------
DROP TABLE IF EXISTS `ro_ticketing`.`driver` ;

CREATE TABLE IF NOT EXISTS `ro_ticketing`.`driver` (
  `Id` INT(11) NOT NULL AUTO_INCREMENT,
  `AppUserId` INT(11) NOT NULL,
  `IsVisible` BIT(1) NOT NULL,
  `IsEnabled` BIT(1) NOT NULL DEFAULT b'1',
  `IsDeleted` BIT(1) NOT NULL DEFAULT b'0',
  `CreatedAt` DATETIME NOT NULL DEFAULT '0001-01-01 00:00:00',
  `CreatedBy` TINYTEXT NULL DEFAULT NULL,
  `UpdatedAt` DATETIME NOT NULL DEFAULT '0001-01-01 00:00:00',
  `UpdatedBy` TINYTEXT NULL DEFAULT NULL,
  PRIMARY KEY (`Id`))
ENGINE = InnoDB
AUTO_INCREMENT = 104
DEFAULT CHARACTER SET = utf8;


-- -----------------------------------------------------
-- Table `ro_ticketing`.`jobtype`
-- -----------------------------------------------------
DROP TABLE IF EXISTS `ro_ticketing`.`jobtype` ;

CREATE TABLE IF NOT EXISTS `ro_ticketing`.`jobtype` (
  `Id` INT(11) NOT NULL AUTO_INCREMENT,
  `Name` VARCHAR(255) NOT NULL,
  `IsVisible` BIT(1) NOT NULL,
  `IsEnabled` BIT(1) NOT NULL,
  `IsDeleted` BIT(1) NOT NULL,
  `CreatedAt` DATETIME NOT NULL DEFAULT '0001-01-01 00:00:00',
  `CreatedBy` TINYTEXT NULL DEFAULT NULL,
  `UpdatedAt` DATETIME NOT NULL DEFAULT '0001-01-01 00:00:00',
  `UpdatedBy` TINYTEXT NULL DEFAULT NULL,
  PRIMARY KEY (`Id`))
ENGINE = InnoDB
AUTO_INCREMENT = 27
DEFAULT CHARACTER SET = utf8;


-- -----------------------------------------------------
-- Table `ro_ticketing`.`market`
-- -----------------------------------------------------
DROP TABLE IF EXISTS `ro_ticketing`.`market` ;

CREATE TABLE IF NOT EXISTS `ro_ticketing`.`market` (
  `id` INT(11) UNSIGNED NOT NULL AUTO_INCREMENT,
  `Name` VARCHAR(255) NOT NULL,
  `description` VARCHAR(100) NOT NULL,
  `IsVisible` BIT(1) NOT NULL,
  `IsEnabled` BIT(1) NOT NULL DEFAULT b'1',
  `IsDeleted` BIT(1) NOT NULL DEFAULT b'0',
  `CreatedAt` DATETIME NOT NULL DEFAULT '0001-01-01 00:00:00',
  `CreatedBy` TINYTEXT NULL DEFAULT NULL,
  `UpdatedAt` DATETIME NOT NULL DEFAULT '0001-01-01 00:00:00',
  `UpdatedBy` TINYTEXT NULL DEFAULT NULL,
  PRIMARY KEY (`id`))
ENGINE = InnoDB
AUTO_INCREMENT = 39
DEFAULT CHARACTER SET = utf8;


-- -----------------------------------------------------
-- Table `ro_ticketing`.`markettax`
-- -----------------------------------------------------
DROP TABLE IF EXISTS `ro_ticketing`.`markettax` ;

CREATE TABLE IF NOT EXISTS `ro_ticketing`.`markettax` (
  `Id` INT(11) NOT NULL AUTO_INCREMENT,
  `Locality` VARCHAR(255) NOT NULL,
  `MarketId` INT(11) NOT NULL,
  `TaxId` INT(11) NOT NULL,
  `IsEnabled` BIT(1) NOT NULL DEFAULT b'1',
  `IsDeleted` BIT(1) NOT NULL DEFAULT b'0',
  `CreatedAt` DATETIME NOT NULL DEFAULT '0001-01-01 00:00:00',
  `CreatedBy` TINYTEXT NULL DEFAULT NULL,
  `UpdatedAt` DATETIME NOT NULL DEFAULT '0001-01-01 00:00:00',
  `UpdatedBy` TINYTEXT NULL DEFAULT NULL,
  PRIMARY KEY USING BTREE (`Id`))
ENGINE = InnoDB
AUTO_INCREMENT = 7
DEFAULT CHARACTER SET = utf8
COLLATE = utf8_bin
ROW_FORMAT = DYNAMIC;


-- -----------------------------------------------------
-- Table `ro_ticketing`.`module`
-- -----------------------------------------------------
DROP TABLE IF EXISTS `ro_ticketing`.`module` ;

CREATE TABLE IF NOT EXISTS `ro_ticketing`.`module` (
  `Id` INT(11) NOT NULL AUTO_INCREMENT,
  `ModuleName` VARCHAR(45) NULL DEFAULT NULL,
  `NormalizedName` VARCHAR(45) NULL DEFAULT NULL,
  `CreatedAt` DATETIME NULL DEFAULT NULL,
  `CreatedBy` TINYTEXT NULL DEFAULT NULL,
  `UpdatedAt` DATETIME NULL DEFAULT NULL,
  `UpdatedBy` TINYTEXT NULL DEFAULT NULL,
  `isEnabled` BIT(1) NULL DEFAULT NULL,
  `isDeleted` BIT(1) NULL DEFAULT NULL,
  PRIMARY KEY USING BTREE (`Id`))
ENGINE = InnoDB
AUTO_INCREMENT = 36
DEFAULT CHARACTER SET = utf8
COLLATE = utf8_bin
ROW_FORMAT = DYNAMIC;


-- -----------------------------------------------------
-- Table `ro_ticketing`.`order`
-- -----------------------------------------------------
DROP TABLE IF EXISTS `ro_ticketing`.`order` ;

CREATE TABLE IF NOT EXISTS `ro_ticketing`.`order` (
  `Id` INT(11) NOT NULL AUTO_INCREMENT,
  `OrderDate` DATETIME NOT NULL,
  `RequestDate` DATETIME NOT NULL,
  `RequestTime` VARCHAR(30) CHARACTER SET 'utf8' COLLATE 'utf8_bin' NOT NULL,
  `OrderStatusId` TINYINT(3) UNSIGNED NOT NULL DEFAULT '0',
  `JobTypeId` SMALLINT(6) NOT NULL,
  `CustomerId` INT(11) NOT NULL,
  `CustomerNotes` VARCHAR(1500) NULL DEFAULT NULL,
  `RigLocationId` INT(11) NOT NULL,
  `RigLocationNotes` VARCHAR(1500) NULL DEFAULT NULL,
  `WellId` INT(11) NOT NULL,
  `WellName` VARCHAR(100) NULL DEFAULT NULL,
  `WellCode` VARCHAR(100) NULL DEFAULT NULL,
  `DriverId` INT(11) NOT NULL,
  `TruckId` INT(11) NOT NULL,
  `AFEPO` VARCHAR(50) CHARACTER SET 'utf8' COLLATE 'utf8_bin' NULL DEFAULT NULL,
  `PointOfContactName` VARCHAR(255) CHARACTER SET 'utf8' COLLATE 'utf8_bin' NULL DEFAULT NULL,
  `PointOfContactNumber` VARCHAR(255) CHARACTER SET 'utf8' COLLATE 'utf8_bin' NULL DEFAULT NULL,
  `MarketId` INT(11) NOT NULL DEFAULT '0',
  `LoadOriginId` INT(11) NOT NULL,
  `SalesRepId` INT(11) NOT NULL,
  `OrderDescription` VARCHAR(1000) CHARACTER SET 'utf8' COLLATE 'utf8_bin' NOT NULL,
  `WellDirection` VARCHAR(1000) CHARACTER SET 'utf8' COLLATE 'utf8_bin' NULL DEFAULT NULL,
  `InternalNotes` VARCHAR(1500) CHARACTER SET 'utf8' COLLATE 'utf8_bin' NULL DEFAULT NULL,
  `ShippingPaperNA` BIT(1) NOT NULL DEFAULT b'0',
  `ShippingPaperExists` BIT(1) NULL DEFAULT b'0',
  `SpecialHandling` VARCHAR(25) NULL DEFAULT NULL,
  `DeliveredDate` DATETIME NOT NULL DEFAULT '0001-01-01 00:00:00',
  `IsEnabled` BIT(1) NOT NULL,
  `IsDeleted` BIT(1) NOT NULL,
  `CreatedAt` DATETIME NOT NULL DEFAULT '0001-01-01 00:00:00',
  `CreatedBy` TINYTEXT CHARACTER SET 'utf8' COLLATE 'utf8_bin' NULL DEFAULT NULL,
  `UpdatedAt` DATETIME NOT NULL DEFAULT '0001-01-01 00:00:00',
  `UpdatedBy` TINYTEXT CHARACTER SET 'utf8' COLLATE 'utf8_bin' NULL DEFAULT NULL,
  PRIMARY KEY (`Id`),
  INDEX `DriverId_idx` (`DriverId` ASC))
ENGINE = InnoDB
AUTO_INCREMENT = 648
DEFAULT CHARACTER SET = utf8;


-- -----------------------------------------------------
-- Table `ro_ticketing`.`orderdirection`
-- -----------------------------------------------------
DROP TABLE IF EXISTS `ro_ticketing`.`orderdirection` ;

CREATE TABLE IF NOT EXISTS `ro_ticketing`.`orderdirection` (
  `Id` INT(11) NOT NULL AUTO_INCREMENT,
  `OrderDirections` VARCHAR(1025) NOT NULL,
  `IsEnabled` BIT(1) NOT NULL,
  `IsDeleted` BIT(1) NOT NULL,
  `CreatedAt` DATETIME NOT NULL DEFAULT '0001-01-01 00:00:00',
  `CreatedBy` TINYTEXT NULL DEFAULT NULL,
  `UpdatedAt` DATETIME NOT NULL DEFAULT '0001-01-01 00:00:00',
  `UpdatedBy` TINYTEXT NULL DEFAULT NULL,
  PRIMARY KEY USING BTREE (`Id`))
ENGINE = InnoDB
AUTO_INCREMENT = 2
DEFAULT CHARACTER SET = utf8
COLLATE = utf8_bin
ROW_FORMAT = DYNAMIC;


-- -----------------------------------------------------
-- Table `ro_ticketing`.`orderlog`
-- -----------------------------------------------------
DROP TABLE IF EXISTS `ro_ticketing`.`orderlog` ;

CREATE TABLE IF NOT EXISTS `ro_ticketing`.`orderlog` (
  `Id` INT(11) NOT NULL AUTO_INCREMENT,
  `OrderId` INT(11) NOT NULL,
  `UserId` VARCHAR(128) NOT NULL,
  `EventType` VARCHAR(255) NULL DEFAULT NULL,
  `Description` VARCHAR(1025) NULL DEFAULT NULL,
  `IsTable` VARCHAR(50) NULL DEFAULT NULL,
  `CreatedTime` DATETIME NULL DEFAULT NULL,
  PRIMARY KEY USING BTREE (`Id`))
ENGINE = InnoDB
AUTO_INCREMENT = 630
DEFAULT CHARACTER SET = utf8
COLLATE = utf8_bin
ROW_FORMAT = DYNAMIC;


-- -----------------------------------------------------
-- Table `ro_ticketing`.`orderstatus`
-- -----------------------------------------------------
DROP TABLE IF EXISTS `ro_ticketing`.`orderstatus` ;

CREATE TABLE IF NOT EXISTS `ro_ticketing`.`orderstatus` (
  `Id` INT(11) NOT NULL AUTO_INCREMENT,
  `Name` VARCHAR(50) NOT NULL,
  `IsEnabled` BIT(1) NOT NULL,
  `IsDeleted` BIT(1) NOT NULL,
  `CreatedAt` DATETIME NOT NULL DEFAULT '0001-01-01 00:00:00',
  `CreatedBy` TINYTEXT NULL DEFAULT NULL,
  `UpdatedAt` DATETIME NOT NULL DEFAULT '0001-01-01 00:00:00',
  `UpdatedBy` TINYTEXT NULL DEFAULT NULL,
  PRIMARY KEY USING BTREE (`Id`))
ENGINE = InnoDB
AUTO_INCREMENT = 7
DEFAULT CHARACTER SET = utf8
COLLATE = utf8_bin
ROW_FORMAT = DYNAMIC;


-- -----------------------------------------------------
-- Table `ro_ticketing`.`paymentterm`
-- -----------------------------------------------------
DROP TABLE IF EXISTS `ro_ticketing`.`paymentterm` ;

CREATE TABLE IF NOT EXISTS `ro_ticketing`.`paymentterm` (
  `Id` INT(11) NOT NULL AUTO_INCREMENT,
  `Name` VARCHAR(35) NOT NULL,
  `IsVisible` BIT(1) NOT NULL,
  `IsEnabled` BIT(1) NULL DEFAULT b'1',
  `IsDeleted` BIT(1) NULL DEFAULT b'0',
  `CreatedAt` DATETIME NULL DEFAULT NULL,
  `CreatedBy` TINYTEXT NULL DEFAULT NULL,
  `UpdatedAt` DATETIME NULL DEFAULT NULL,
  `UpdatedBy` TINYTEXT NULL DEFAULT NULL,
  PRIMARY KEY USING BTREE (`Id`))
ENGINE = InnoDB
AUTO_INCREMENT = 7
DEFAULT CHARACTER SET = utf8
COLLATE = utf8_bin
ROW_FORMAT = DYNAMIC;


-- -----------------------------------------------------
-- Table `ro_ticketing`.`permission`
-- -----------------------------------------------------
DROP TABLE IF EXISTS `ro_ticketing`.`permission` ;

CREATE TABLE IF NOT EXISTS `ro_ticketing`.`permission` (
  `Id` INT(11) NOT NULL AUTO_INCREMENT,
  `ModuleId` INT(11) NULL DEFAULT NULL,
  `RoleId` VARCHAR(128) NULL DEFAULT NULL,
  `IsRead` TINYINT(1) NULL DEFAULT NULL,
  `IsCreate` TINYINT(1) NULL DEFAULT NULL,
  `IsUpdate` TINYINT(1) NULL DEFAULT NULL,
  `IsDelete` TINYINT(1) NULL DEFAULT NULL,
  `CreatedAt` DATETIME NULL DEFAULT NULL,
  `CreatedBy` TINYTEXT NULL DEFAULT NULL,
  `UpdatedAt` DATETIME NULL DEFAULT NULL,
  `UpdatedBy` TINYTEXT NULL DEFAULT NULL,
  PRIMARY KEY USING BTREE (`Id`),
  INDEX `ModuleId_idx` USING BTREE (`ModuleId`),
  INDEX `RoleId_idx` USING BTREE (`RoleId`),
  CONSTRAINT `ModuleId`
    FOREIGN KEY (`ModuleId`)
    REFERENCES `ro_ticketing`.`module` (`Id`))
ENGINE = InnoDB
AUTO_INCREMENT = 195
DEFAULT CHARACTER SET = utf8
COLLATE = utf8_bin
ROW_FORMAT = DYNAMIC;


-- -----------------------------------------------------
-- Table `ro_ticketing`.`product`
-- -----------------------------------------------------
DROP TABLE IF EXISTS `ro_ticketing`.`product` ;

CREATE TABLE IF NOT EXISTS `ro_ticketing`.`product` (
  `Id` INT(11) NOT NULL AUTO_INCREMENT,
  `Name` VARCHAR(255) NOT NULL,
  `ProductCategoryId` INT(11) NOT NULL,
  `UnitId` INT(11) NOT NULL,
  `UnitCost` DECIMAL(13,5) NOT NULL,
  `UnitPrice` DECIMAL(13,5) NOT NULL,
  `IsIncludedInReport` BIT(1) NOT NULL DEFAULT b'1',
  `IsVisible` BIT(1) NOT NULL,
  `IsEnabled` BIT(1) NOT NULL DEFAULT b'1',
  `IsDeleted` BIT(1) NOT NULL DEFAULT b'0',
  `CreatedAt` DATETIME NOT NULL DEFAULT '0001-01-01 00:00:00',
  `CreatedBy` TINYTEXT NULL DEFAULT NULL,
  `UpdatedAt` DATETIME NOT NULL DEFAULT '0001-01-01 00:00:00',
  `UpdatedBy` TINYTEXT NULL DEFAULT NULL,
  PRIMARY KEY (`Id`))
ENGINE = InnoDB
AUTO_INCREMENT = 95
DEFAULT CHARACTER SET = utf8;


-- -----------------------------------------------------
-- Table `ro_ticketing`.`productcategory`
-- -----------------------------------------------------
DROP TABLE IF EXISTS `ro_ticketing`.`productcategory` ;

CREATE TABLE IF NOT EXISTS `ro_ticketing`.`productcategory` (
  `Id` INT(11) NOT NULL AUTO_INCREMENT,
  `Name` VARCHAR(255) NOT NULL,
  `IsVisible` BIT(1) NOT NULL,
  `IsEnabled` BIT(1) NOT NULL DEFAULT b'1',
  `IsDeleted` BIT(1) NOT NULL DEFAULT b'0',
  `CreatedAt` DATETIME NOT NULL DEFAULT '0001-01-01 00:00:00',
  `CreatedBy` TINYTEXT NULL DEFAULT NULL,
  `UpdatedAt` DATETIME NOT NULL DEFAULT '0001-01-01 00:00:00',
  `UpdatedBy` TINYTEXT NULL DEFAULT NULL,
  PRIMARY KEY USING BTREE (`Id`))
ENGINE = InnoDB
AUTO_INCREMENT = 21
DEFAULT CHARACTER SET = utf8
COLLATE = utf8_bin
ROW_FORMAT = DYNAMIC;


-- -----------------------------------------------------
-- Table `ro_ticketing`.`producttax`
-- -----------------------------------------------------
DROP TABLE IF EXISTS `ro_ticketing`.`producttax` ;

CREATE TABLE IF NOT EXISTS `ro_ticketing`.`producttax` (
  `Id` INT(11) NOT NULL AUTO_INCREMENT,
  `TaxId` INT(11) NOT NULL,
  `ProductId` INT(11) NOT NULL,
  `IsDeleted` BIT(1) NOT NULL,
  `IsEnabled` BIT(1) NOT NULL,
  `CreatedAt` DATETIME NOT NULL DEFAULT '0001-01-01 00:00:00',
  `CreatedBy` TINYTEXT CHARACTER SET 'utf8' COLLATE 'utf8_bin' NULL DEFAULT NULL,
  `UpdatedAt` DATETIME NOT NULL DEFAULT '0001-01-01 00:00:00',
  `UpdatedBy` TINYTEXT CHARACTER SET 'utf8' COLLATE 'utf8_bin' NULL DEFAULT NULL,
  PRIMARY KEY USING BTREE (`Id`),
  INDEX `TaxId_idx` (`TaxId` ASC),
  INDEX `ProductId_idx` (`ProductId` ASC))
ENGINE = InnoDB
AUTO_INCREMENT = 62
DEFAULT CHARACTER SET = utf8mb4
COLLATE = utf8mb4_bin
ROW_FORMAT = DYNAMIC;


-- -----------------------------------------------------
-- Table `ro_ticketing`.`riglocation`
-- -----------------------------------------------------
DROP TABLE IF EXISTS `ro_ticketing`.`riglocation` ;

CREATE TABLE IF NOT EXISTS `ro_ticketing`.`riglocation` (
  `Id` INT(11) NOT NULL AUTO_INCREMENT,
  `CustomerId` INT(11) NOT NULL,
  `Name` VARCHAR(255) NOT NULL,
  `Note` VARCHAR(1500) NULL DEFAULT NULL,
  `IsVisible` BIT(1) NOT NULL,
  `IsEnabled` BIT(1) NOT NULL,
  `IsDeleted` BIT(1) NOT NULL,
  `CreatedAt` DATETIME NOT NULL DEFAULT '0001-01-01 00:00:00',
  `CreatedBy` TINYTEXT NULL DEFAULT NULL,
  `UpdatedAt` DATETIME NOT NULL DEFAULT '0001-01-01 00:00:00',
  `UpdatedBy` TINYTEXT NULL DEFAULT NULL,
  PRIMARY KEY (`Id`))
ENGINE = InnoDB
AUTO_INCREMENT = 43
DEFAULT CHARACTER SET = utf8;


-- -----------------------------------------------------
-- Table `ro_ticketing`.`riglocationnote`
-- -----------------------------------------------------
DROP TABLE IF EXISTS `ro_ticketing`.`riglocationnote` ;

CREATE TABLE IF NOT EXISTS `ro_ticketing`.`riglocationnote` (
  `Id` INT(11) NOT NULL AUTO_INCREMENT,
  `RigLocationId` INT(11) NOT NULL,
  `RigNote` VARCHAR(1025) NULL DEFAULT NULL,
  `IsEnabled` BIT(1) NOT NULL,
  `IsDeleted` BIT(1) NOT NULL,
  `CreatedAt` DATETIME NOT NULL DEFAULT '0001-01-01 00:00:00',
  `CreatedBy` TINYTEXT NULL DEFAULT NULL,
  `UpdatedAt` DATETIME NOT NULL DEFAULT '0001-01-01 00:00:00',
  `UpdatedBy` TINYTEXT NULL DEFAULT NULL,
  PRIMARY KEY USING BTREE (`Id`))
ENGINE = InnoDB
AUTO_INCREMENT = 3
DEFAULT CHARACTER SET = utf8
COLLATE = utf8_bin
ROW_FORMAT = DYNAMIC;


-- -----------------------------------------------------
-- Table `ro_ticketing`.`salesrep`
-- -----------------------------------------------------
DROP TABLE IF EXISTS `ro_ticketing`.`salesrep` ;

CREATE TABLE IF NOT EXISTS `ro_ticketing`.`salesrep` (
  `Id` INT(11) NOT NULL AUTO_INCREMENT,
  `AppUserId` INT(11) NOT NULL,
  `IsVisible` BIT(1) NULL DEFAULT b'1',
  `Notes` VARCHAR(1500) NULL DEFAULT NULL,
  `IsEnabled` BIT(1) NOT NULL DEFAULT b'1',
  `IsDeleted` BIT(1) NOT NULL DEFAULT b'0',
  `CreatedAt` DATETIME NOT NULL DEFAULT '0001-01-01 00:00:00',
  `CreatedBy` TINYTEXT CHARACTER SET 'utf8' NULL DEFAULT NULL,
  `UpdatedAt` DATETIME NOT NULL DEFAULT '0001-01-01 00:00:00',
  `UpdatedBy` TINYTEXT CHARACTER SET 'utf8' NULL DEFAULT NULL,
  PRIMARY KEY USING BTREE (`Id`))
ENGINE = InnoDB
AUTO_INCREMENT = 10
DEFAULT CHARACTER SET = utf8
COLLATE = utf8_bin
ROW_FORMAT = DYNAMIC;


-- -----------------------------------------------------
-- Table `ro_ticketing`.`setting`
-- -----------------------------------------------------
DROP TABLE IF EXISTS `ro_ticketing`.`setting` ;

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
AUTO_INCREMENT = 14
DEFAULT CHARACTER SET = utf8;


-- -----------------------------------------------------
-- Table `ro_ticketing`.`shippingpaper`
-- -----------------------------------------------------
DROP TABLE IF EXISTS `ro_ticketing`.`shippingpaper` ;

CREATE TABLE IF NOT EXISTS `ro_ticketing`.`shippingpaper` (
  `Id` INT(11) NOT NULL AUTO_INCREMENT,
  `MarketId` INT(11) NOT NULL,
  `Name` VARCHAR(255) NULL DEFAULT NULL,
  `Content` MEDIUMTEXT NOT NULL,
  `IsEnabled` BIT(1) NULL DEFAULT b'1',
  `IsDeleted` TINYINT(1) NULL DEFAULT NULL,
  `CreatedAt` DATETIME NULL DEFAULT NULL,
  `CreatedBy` TINYTEXT NULL DEFAULT NULL,
  `UpdatedAt` DATETIME NULL DEFAULT NULL,
  `UpdatedBy` TINYTEXT NULL DEFAULT NULL,
  PRIMARY KEY USING BTREE (`Id`, `MarketId`))
ENGINE = InnoDB
AUTO_INCREMENT = 8
DEFAULT CHARACTER SET = utf8
COLLATE = utf8_bin
ROW_FORMAT = DYNAMIC;


-- -----------------------------------------------------
-- Table `ro_ticketing`.`shippingpaperimage`
-- -----------------------------------------------------
DROP TABLE IF EXISTS `ro_ticketing`.`shippingpaperimage` ;

CREATE TABLE IF NOT EXISTS `ro_ticketing`.`shippingpaperimage` (
  `Id` INT(11) NOT NULL AUTO_INCREMENT,
  `TicketId` INT(11) NOT NULL,
  `FileName` VARCHAR(128) NOT NULL,
  `FilePath` VARCHAR(256) NOT NULL,
  `IsEnabled` BIT(1) NOT NULL DEFAULT b'1',
  `IsDeleted` BIT(1) NOT NULL,
  `CreatedAt` DATETIME NOT NULL,
  `CreatedBy` TINYTEXT NULL DEFAULT NULL,
  `UpdatedAt` DATETIME NOT NULL,
  `UpdatedBy` TINYTEXT NULL DEFAULT NULL,
  PRIMARY KEY USING BTREE (`Id`))
ENGINE = InnoDB
AUTO_INCREMENT = 51
DEFAULT CHARACTER SET = utf8
COLLATE = utf8_bin
ROW_FORMAT = DYNAMIC;


-- -----------------------------------------------------
-- Table `ro_ticketing`.`tax`
-- -----------------------------------------------------
DROP TABLE IF EXISTS `ro_ticketing`.`tax` ;

CREATE TABLE IF NOT EXISTS `ro_ticketing`.`tax` (
  `Id` INT(11) NOT NULL AUTO_INCREMENT,
  `Name` VARCHAR(255) NOT NULL,
  `MarketId` INT(11) NULL DEFAULT NULL,
  `TaxType` VARCHAR(10) NULL DEFAULT NULL,
  `TaxValue` DECIMAL(13,4) NULL DEFAULT '0.0000',
  `IsEnabled` BIT(1) NULL DEFAULT b'1',
  `IsDeleted` BIT(1) NULL DEFAULT b'0',
  `CreatedAt` DATETIME NULL DEFAULT '0001-01-01 00:00:00',
  `CreatedBy` TINYTEXT NULL DEFAULT NULL,
  `UpdatedAt` DATETIME NULL DEFAULT '0001-01-01 00:00:00',
  `UpdatedBy` TINYTEXT NULL DEFAULT NULL,
  PRIMARY KEY USING BTREE (`Id`))
ENGINE = InnoDB
AUTO_INCREMENT = 21
DEFAULT CHARACTER SET = utf8
COLLATE = utf8_bin
ROW_FORMAT = DYNAMIC;


-- -----------------------------------------------------
-- Table `ro_ticketing`.`ticketimage`
-- -----------------------------------------------------
DROP TABLE IF EXISTS `ro_ticketing`.`ticketimage` ;

CREATE TABLE IF NOT EXISTS `ro_ticketing`.`ticketimage` (
  `Id` INT(11) NOT NULL AUTO_INCREMENT,
  `TicketId` INT(11) NOT NULL,
  `TicketImageType` VARCHAR(55) NULL DEFAULT NULL,
  `FileName` VARCHAR(128) NULL DEFAULT NULL,
  `UserId` VARCHAR(128) NULL DEFAULT NULL,
  `IsMatch` BIT(1) NULL DEFAULT b'0',
  `IsEnabled` BIT(1) NOT NULL DEFAULT b'1',
  `IsDeleted` BIT(1) NOT NULL DEFAULT b'0',
  `CreatedAt` DATETIME NOT NULL DEFAULT '0001-01-01 00:00:00',
  `CreatedBy` VARCHAR(45) NULL DEFAULT NULL,
  `UpdatedAt` DATETIME NOT NULL DEFAULT '0001-01-01 00:00:00',
  `UpdatedBy` VARCHAR(45) NULL DEFAULT NULL,
  PRIMARY KEY USING BTREE (`Id`))
ENGINE = InnoDB
DEFAULT CHARACTER SET = utf8
COLLATE = utf8_bin
ROW_FORMAT = DYNAMIC;


-- -----------------------------------------------------
-- Table `ro_ticketing`.`ticketpaper`
-- -----------------------------------------------------
DROP TABLE IF EXISTS `ro_ticketing`.`ticketpaper` ;

CREATE TABLE IF NOT EXISTS `ro_ticketing`.`ticketpaper` (
  `Id` INT(11) NOT NULL AUTO_INCREMENT,
  `MarketId` INT(11) NOT NULL,
  `Name` VARCHAR(255) NULL DEFAULT NULL,
  `Content` MEDIUMTEXT NOT NULL,
  `IsEnabled` BIT(1) NULL DEFAULT b'1',
  `IsDeleted` TINYINT(1) NULL DEFAULT NULL,
  `CreatedAt` DATETIME NULL DEFAULT NULL,
  `CreatedBy` TINYTEXT NULL DEFAULT NULL,
  `UpdatedAt` DATETIME NULL DEFAULT NULL,
  `UpdatedBy` TINYTEXT NULL DEFAULT NULL,
  PRIMARY KEY USING BTREE (`Id`, `MarketId`))
ENGINE = InnoDB
AUTO_INCREMENT = 8
DEFAULT CHARACTER SET = utf8
COLLATE = utf8_bin
ROW_FORMAT = DYNAMIC;


-- -----------------------------------------------------
-- Table `ro_ticketing`.`ticketpaperimage`
-- -----------------------------------------------------
DROP TABLE IF EXISTS `ro_ticketing`.`ticketpaperimage` ;

CREATE TABLE IF NOT EXISTS `ro_ticketing`.`ticketpaperimage` (
  `Id` INT(11) NOT NULL AUTO_INCREMENT,
  `TicketId` INT(11) NOT NULL,
  `FileName` VARCHAR(128) NOT NULL,
  `FilePath` VARCHAR(256) NOT NULL,
  `IsEnabled` BIT(1) NOT NULL DEFAULT b'1',
  `IsDeleted` BIT(1) NOT NULL,
  `CreatedAt` DATETIME NOT NULL,
  `CreatedBy` TINYTEXT NULL DEFAULT NULL,
  `UpdatedAt` DATETIME NOT NULL,
  `UpdatedBy` TINYTEXT NULL DEFAULT NULL,
  PRIMARY KEY USING BTREE (`Id`))
ENGINE = InnoDB
AUTO_INCREMENT = 17
DEFAULT CHARACTER SET = utf8
COLLATE = utf8_bin
ROW_FORMAT = DYNAMIC;


-- -----------------------------------------------------
-- Table `ro_ticketing`.`ticketproduct`
-- -----------------------------------------------------
DROP TABLE IF EXISTS `ro_ticketing`.`ticketproduct` ;

CREATE TABLE IF NOT EXISTS `ro_ticketing`.`ticketproduct` (
  `Id` INT(11) NOT NULL AUTO_INCREMENT,
  `TicketId` INT(11) NOT NULL,
  `ProductId` INT(11) NOT NULL,
  `Quantity` DECIMAL(13,5) NOT NULL DEFAULT '1.00000',
  `UnitId` INT(11) NOT NULL,
  `Price` DECIMAL(13,5) NULL DEFAULT NULL,
  `IsEnabled` BIT(1) NOT NULL DEFAULT b'1',
  `IsDeleted` BIT(1) NOT NULL DEFAULT b'0',
  `CreatedAt` DATETIME NOT NULL DEFAULT '0001-01-01 00:00:00',
  `CreatedBy` TINYTEXT NULL DEFAULT NULL,
  `UpdatedAt` DATETIME NOT NULL DEFAULT '0001-01-01 00:00:00',
  `UpdatedBy` TINYTEXT NULL DEFAULT NULL,
  PRIMARY KEY USING BTREE (`Id`))
ENGINE = InnoDB
AUTO_INCREMENT = 124
DEFAULT CHARACTER SET = utf8
COLLATE = utf8_bin
ROW_FORMAT = DYNAMIC;


-- -----------------------------------------------------
-- Table `ro_ticketing`.`tickettax`
-- -----------------------------------------------------
DROP TABLE IF EXISTS `ro_ticketing`.`tickettax` ;

CREATE TABLE IF NOT EXISTS `ro_ticketing`.`tickettax` (
  `Id` INT(11) NOT NULL AUTO_INCREMENT,
  `TicketId` INT(11) NOT NULL,
  `TaxId` INT(11) NOT NULL DEFAULT '0',
  `TaxDescription` VARCHAR(255) NOT NULL,
  `TaxType` VARCHAR(10) NOT NULL,
  `TaxValue` DECIMAL(13,5) NOT NULL,
  `TaxAmount` DECIMAL(13,5) NOT NULL,
  `IsEnabled` BIT(1) NOT NULL DEFAULT b'1',
  `IsDeleted` BIT(1) NOT NULL DEFAULT b'0',
  `CreatedAt` DATETIME NOT NULL DEFAULT '0001-01-01 00:00:00',
  `CreatedBy` TINYTEXT NULL DEFAULT NULL,
  `UpdatedAt` DATETIME NOT NULL DEFAULT '0001-01-01 00:00:00',
  `UpdatedBy` TINYTEXT NULL DEFAULT NULL,
  PRIMARY KEY USING BTREE (`Id`))
ENGINE = InnoDB
AUTO_INCREMENT = 24
DEFAULT CHARACTER SET = utf8
ROW_FORMAT = DYNAMIC;


-- -----------------------------------------------------
-- Table `ro_ticketing`.`truck`
-- -----------------------------------------------------
DROP TABLE IF EXISTS `ro_ticketing`.`truck` ;

CREATE TABLE IF NOT EXISTS `ro_ticketing`.`truck` (
  `Id` INT(11) NOT NULL AUTO_INCREMENT,
  `Name` VARCHAR(255) NOT NULL,
  `IsVisible` BIT(1) NOT NULL,
  `IsEnabled` BIT(1) NOT NULL DEFAULT b'1',
  `IsDeleted` BIT(1) NOT NULL,
  `CreatedAt` DATETIME NOT NULL DEFAULT '0001-01-01 00:00:00',
  `CreatedBy` TINYTEXT NULL DEFAULT NULL,
  `UpdatedAt` DATETIME NOT NULL DEFAULT '0001-01-01 00:00:00',
  `UpdatedBy` TINYTEXT NULL DEFAULT NULL,
  PRIMARY KEY (`Id`))
ENGINE = InnoDB
AUTO_INCREMENT = 223
DEFAULT CHARACTER SET = utf8;


-- -----------------------------------------------------
-- Table `ro_ticketing`.`unit`
-- -----------------------------------------------------
DROP TABLE IF EXISTS `ro_ticketing`.`unit` ;

CREATE TABLE IF NOT EXISTS `ro_ticketing`.`unit` (
  `id` INT(11) NOT NULL AUTO_INCREMENT,
  `name` VARCHAR(255) NULL DEFAULT NULL,
  `IsVisible` BIT(1) NOT NULL,
  `isEnabled` BIT(1) NULL DEFAULT b'1',
  `IsDeleted` BIT(1) NULL DEFAULT b'0',
  `CreatedAt` DATETIME NULL DEFAULT NULL,
  `CreatedBy` TINYTEXT NULL DEFAULT NULL,
  `UpdatedAt` DATETIME NULL DEFAULT NULL,
  `UpdatedBy` TINYTEXT NULL DEFAULT NULL,
  PRIMARY KEY USING BTREE (`id`))
ENGINE = InnoDB
AUTO_INCREMENT = 10
DEFAULT CHARACTER SET = utf8
COLLATE = utf8_bin
ROW_FORMAT = DYNAMIC;


-- -----------------------------------------------------
-- Table `ro_ticketing`.`vehicle`
-- -----------------------------------------------------
DROP TABLE IF EXISTS `ro_ticketing`.`vehicle` ;

CREATE TABLE IF NOT EXISTS `ro_ticketing`.`vehicle` (
  `Id` INT(11) NOT NULL AUTO_INCREMENT,
  `Name` VARCHAR(255) NOT NULL,
  `Latitude` FLOAT NOT NULL,
  `Longitude` FLOAT NOT NULL,
  `IsEnabled` BIT(1) NOT NULL DEFAULT b'1',
  `IsDeleted` BIT(1) NOT NULL DEFAULT b'0',
  `CreatedAt` DATETIME NOT NULL,
  `CreatedBy` TINYTEXT NULL DEFAULT NULL,
  `UpdatedAt` DATETIME NOT NULL,
  `UpdatedBy` TINYTEXT NULL DEFAULT NULL,
  PRIMARY KEY USING BTREE (`Id`))
ENGINE = InnoDB
AUTO_INCREMENT = 3
DEFAULT CHARACTER SET = utf8
COLLATE = utf8_bin
ROW_FORMAT = DYNAMIC;


-- -----------------------------------------------------
-- Table `ro_ticketing`.`well`
-- -----------------------------------------------------
DROP TABLE IF EXISTS `ro_ticketing`.`well` ;

CREATE TABLE IF NOT EXISTS `ro_ticketing`.`well` (
  `Id` INT(11) NOT NULL AUTO_INCREMENT,
  `Name` VARCHAR(255) NOT NULL,
  `RigLocationId` INT(11) NOT NULL,
  `Direction` VARCHAR(1000) NULL DEFAULT NULL,
  `IsVisible` BIT(1) NOT NULL,
  `IsEnabled` BIT(1) NOT NULL,
  `IsDeleted` BIT(1) NOT NULL,
  `CreatedAt` DATETIME NOT NULL DEFAULT '0001-01-01 00:00:00',
  `CreatedBy` TINYTEXT NULL DEFAULT NULL,
  `UpdatedAt` DATETIME NOT NULL DEFAULT '0001-01-01 00:00:00',
  `UpdatedBy` TINYTEXT NULL DEFAULT NULL,
  PRIMARY KEY (`Id`))
ENGINE = InnoDB
AUTO_INCREMENT = 52
DEFAULT CHARACTER SET = utf8;


SET SQL_MODE=@OLD_SQL_MODE;
SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS;
SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS;
