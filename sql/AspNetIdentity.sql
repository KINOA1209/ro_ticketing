use ro_ticketing;

truncate table ro_ticketing.appuser;
drop table `aspnetroleclaims`;
drop table `aspnetuserroles`;
drop table `aspnetroles`;
drop table `aspnetuserclaims`;
drop table `aspnetuserlogins`;
drop table `aspnetusertokens`;
drop table `aspnetusers`;


CREATE TABLE `aspnetroles` (
  `Id` varchar(128) CHARACTER SET utf8 COLLATE utf8_bin NOT NULL,
  `ConcurrencyStamp` text CHARACTER SET utf8 COLLATE utf8_bin,
  `Name` varchar(128) CHARACTER SET utf8 COLLATE utf8_bin DEFAULT NULL,
  `NormalizedName` varchar(256) CHARACTER SET utf8 COLLATE utf8_bin DEFAULT NULL,
  PRIMARY KEY (`Id`),
  UNIQUE KEY `RoleNameIndex` (`NormalizedName`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_bin;

CREATE TABLE `aspnetroleclaims` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `ClaimType` text CHARACTER SET utf8 COLLATE utf8_bin,
  `ClaimValue` text CHARACTER SET utf8 COLLATE utf8_bin,
  `RoleId` varchar(128) CHARACTER SET utf8 COLLATE utf8_bin NOT NULL,
  PRIMARY KEY (`Id`),
  KEY `IX_AspNetRoleClaims_RoleId` (`RoleId`),
  CONSTRAINT `FK_AspNetRoleClaims_AspNetRoles_RoleId` FOREIGN KEY (`RoleId`) REFERENCES `aspnetroles` (`id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_bin;

CREATE TABLE `aspnetusers` (
  `Id` varchar(128) CHARACTER SET utf8 COLLATE utf8_bin NOT NULL,
  `AccessFailedCount` int(11) NOT NULL,
  `ConcurrencyStamp` text CHARACTER SET utf8 COLLATE utf8_bin,
  `Email` varchar(256) CHARACTER SET utf8 COLLATE utf8_bin DEFAULT NULL,
  `EmailConfirmed` bit(1) NOT NULL,
  `LockoutEnabled` bit(1) NOT NULL,
  `LockoutEnd` timestamp NULL DEFAULT NULL,
  `NormalizedEmail` varchar(256) CHARACTER SET utf8 COLLATE utf8_bin DEFAULT NULL,
  `NormalizedUserName` varchar(256) CHARACTER SET utf8 COLLATE utf8_bin DEFAULT NULL,
  `PasswordHash` text CHARACTER SET utf8 COLLATE utf8_bin,
  `PhoneNumber` text CHARACTER SET utf8 COLLATE utf8_bin,
  `PhoneNumberConfirmed` bit(1) NOT NULL,
  `SecurityStamp` text CHARACTER SET utf8 COLLATE utf8_bin,
  `TwoFactorEnabled` bit(1) NOT NULL,
  `UserName` varchar(128) CHARACTER SET utf8 COLLATE utf8_bin DEFAULT NULL,
  PRIMARY KEY (`Id`),
  UNIQUE KEY `UserNameIndex` (`NormalizedUserName`),
  KEY `EmailIndex` (`NormalizedEmail`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_bin;

CREATE TABLE `aspnetuserclaims` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `ClaimType` text CHARACTER SET utf8 COLLATE utf8_bin,
  `ClaimValue` text CHARACTER SET utf8 COLLATE utf8_bin,
  `UserId` varchar(128) CHARACTER SET utf8 COLLATE utf8_bin NOT NULL,
  PRIMARY KEY (`Id`),
  KEY `IX_AspNetUserClaims_UserId` (`UserId`),
  CONSTRAINT `FK_AspNetUserClaims_AspNetUsers_UserId` FOREIGN KEY (`UserId`) REFERENCES `aspnetusers` (`id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_bin;

CREATE TABLE `aspnetuserlogins` (
  `LoginProvider` varchar(127) CHARACTER SET utf8 COLLATE utf8_bin NOT NULL,
  `ProviderKey` varchar(127) CHARACTER SET utf8 COLLATE utf8_bin NOT NULL,
  `ProviderDisplayName` text CHARACTER SET utf8 COLLATE utf8_bin,
  `UserId` varchar(128) CHARACTER SET utf8 COLLATE utf8_bin NOT NULL,
  PRIMARY KEY (`LoginProvider`,`ProviderKey`),
  KEY `IX_AspNetUserLogins_UserId` (`UserId`),
  CONSTRAINT `FK_AspNetUserLogins_AspNetUsers_UserId` FOREIGN KEY (`UserId`) REFERENCES `aspnetusers` (`id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_bin;

CREATE TABLE `aspnetuserroles` (
  `UserId` varchar(128) CHARACTER SET utf8 COLLATE utf8_bin NOT NULL,
  `RoleId` varchar(128) CHARACTER SET utf8 COLLATE utf8_bin NOT NULL,
  PRIMARY KEY (`UserId`,`RoleId`),
  KEY `IX_AspNetUserRoles_RoleId` (`RoleId`),
  CONSTRAINT `FK_AspNetUserRoles_AspNetRoles_RoleId` FOREIGN KEY (`RoleId`) REFERENCES `aspnetroles` (`id`) ON DELETE CASCADE,
  CONSTRAINT `FK_AspNetUserRoles_AspNetUsers_UserId` FOREIGN KEY (`UserId`) REFERENCES `aspnetusers` (`id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_bin;


CREATE TABLE `aspnetusertokens` (
  `UserId` varchar(128) CHARACTER SET utf8 COLLATE utf8_bin NOT NULL,
  `LoginProvider` varchar(128) CHARACTER SET utf8 COLLATE utf8_bin NOT NULL,
  `Name` varchar(128) CHARACTER SET utf8 COLLATE utf8_bin NOT NULL,
  `Value` text CHARACTER SET utf8 COLLATE utf8_bin,
  PRIMARY KEY (`UserId`,`LoginProvider`,`Name`),
  CONSTRAINT `FK_AspNetUserTokens_AspNetUsers_UserId` FOREIGN KEY (`UserId`) REFERENCES `aspnetusers` (`id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8 COLLATE=utf8_bin;
