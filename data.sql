-- MySQL dump 10.13  Distrib 5.7.17, for Win64 (x86_64)
--
-- Host: localhost    Database: element_world
-- ------------------------------------------------------
-- Server version	5.6.36-log

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8 */;
/*!40103 SET @OLD_TIME_ZONE=@@TIME_ZONE */;
/*!40103 SET TIME_ZONE='+00:00' */;
/*!40014 SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;
/*!40111 SET @OLD_SQL_NOTES=@@SQL_NOTES, SQL_NOTES=0 */;

--
-- Table structure for table `bags`
--

DROP TABLE IF EXISTS `bags`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `bags` (
  `id` int(10) unsigned NOT NULL AUTO_INCREMENT,
  `uid` int(11) NOT NULL,
  `bagtype` int(11) NOT NULL,
  `bagsize` int(11) NOT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `bags`
--

LOCK TABLES `bags` WRITE;
/*!40000 ALTER TABLE `bags` DISABLE KEYS */;
/*!40000 ALTER TABLE `bags` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `roles`
--

DROP TABLE IF EXISTS `roles`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `roles` (
  `id` int(10) unsigned NOT NULL AUTO_INCREMENT,
  `uid` int(10) unsigned NOT NULL,
  `rolename` varchar(16) NOT NULL,
  `class` int(1) NOT NULL,
  `level` int(1) NOT NULL DEFAULT '1',
  `sex` int(1) NOT NULL,
  `faceid` int(1) NOT NULL,
  `hairid` int(1) NOT NULL,
  `earid` int(1) NOT NULL,
  `tailid` int(1) NOT NULL,
  `showclassic` int(1) NOT NULL DEFAULT '0',
  `status` int(1) NOT NULL DEFAULT '1',
  `createtime` int(11) NOT NULL,
  `lastlogintime` int(11) NOT NULL,
  `deletetime` int(11) NOT NULL DEFAULT '0',
  `mapid` int(11) NOT NULL DEFAULT '401',
  `posx` float NOT NULL DEFAULT '368',
  `posy` float NOT NULL DEFAULT '481',
  `posz` float NOT NULL DEFAULT '348',
  `direction` int(1) NOT NULL DEFAULT '0',
  `fashionmode` tinyint(1) NOT NULL DEFAULT '0',
  `fashionhead` int(10) unsigned NOT NULL DEFAULT '0',
  `fashioncloth` int(10) unsigned NOT NULL DEFAULT '0',
  `fashionshoes` int(10) unsigned NOT NULL DEFAULT '0',
  `fashionweapon` int(10) unsigned NOT NULL DEFAULT '0',
  `powerid` int(1) unsigned NOT NULL DEFAULT '0',
  PRIMARY KEY (`id`),
  UNIQUE KEY `rolename_UNIQUE` (`rolename`)
) ENGINE=InnoDB AUTO_INCREMENT=27 DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `roles`
--

LOCK TABLES `roles` WRITE;
/*!40000 ALTER TABLE `roles` DISABLE KEYS */;
INSERT INTO `roles` VALUES (15,2,'1234',39,1,1,0,0,0,0,0,1,1500193844,1502793918,0,401,368,481,348,0,1,66911,66910,66912,0,0),(21,1,'Áîª‰∫Ü‰∏™ÂΩ±',83,1,1,0,0,0,0,0,1,1502796677,1502868243,0,401,368,481,348,0,1,66911,66910,66912,0,0),(22,1,'233',83,160,0,0,0,0,0,1,3,1502868215,1502868215,1631783843,401,368,481,348,0,1,66906,66904,66909,0,2),(23,7,'Âº†‰∏â',83,1,0,0,1,0,0,1,1,1632507358,0,0,401,368,481,348,0,1,66906,66904,66909,0,0),(24,7,'ÊùéÂõõ',83,1,1,0,0,0,0,0,1,1632507449,0,0,401,368,481,348,0,1,66911,66910,66912,0,0);
/*!40000 ALTER TABLE `roles` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `users`
--

DROP TABLE IF EXISTS `users`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `users` (
  `id` int(10) unsigned NOT NULL AUTO_INCREMENT,
  `uname` varchar(20) NOT NULL,
  `upwd` varbinary(64) NOT NULL,
  `isgm` bit(1) NOT NULL DEFAULT b'0',
  `cash` int(11) NOT NULL DEFAULT '0',
  `online` tinyint(1) NOT NULL DEFAULT '0',
  PRIMARY KEY (`id`),
  UNIQUE KEY `uname_UNIQUE` (`uname`)
) ENGINE=InnoDB AUTO_INCREMENT=10 DEFAULT CHARSET=utf8;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `users`
--

LOCK TABLES `users` WRITE;
/*!40000 ALTER TABLE `users` DISABLE KEYS */;
INSERT INTO `users` VALUES (1,'1','eΩC\Ÿ ¶\‡,ô\nÇe-\ ','\0',0,0),(2,'2','∂\◊g\“¯\Ì]!§KXÜhπ','\0',0,0),(6,'4','4ß_L\Íë	P|¨\ÿ\‚ÚÆ¸','\0',0,1),(7,'3','¡jS ˙GU0\ŸX<4˝5nı','\0',0,0),(9,'polison','\ÌU¯+o≥W£Æ\Ÿ|<˘¿','\0',0,1);
/*!40000 ALTER TABLE `users` ENABLE KEYS */;
UNLOCK TABLES;
/*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;

-- Dump completed on 2021-09-24 18:24:08
