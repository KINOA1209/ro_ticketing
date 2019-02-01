/*
SET FOREIGN_KEY_CHECKS = 0; 
truncate table ro_ticketing.audit;
truncate table ro_ticketing.order;
truncate table ro_ticketing.orderlog;
truncate table ro_ticketing.shippingpaperimage;
truncate table ro_ticketing.ticketpaperimage;
truncate table ro_ticketing.ticketproduct;
truncate table ro_ticketing.tickettax;
SET FOREIGN_KEY_CHECKS = 1;
*/


truncate table ro_ticketing.audit;
truncate table ro_ticketing.order;
truncate table ro_ticketing.orderlog;
truncate table ro_ticketing.shippingpaperimage;
truncate table ro_ticketing.ticketpaperimage;
truncate table ro_ticketing.ticketproduct;
truncate table ro_ticketing.tickettax;
ALTER TABLE ro_ticketing.order AUTO_INCREMENT=2000000;