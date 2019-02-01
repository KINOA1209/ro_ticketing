
SELECT DISTINCT CONCAT('INSERT INTO ro_ticketing.PRODUCT (name) VALUE (''',unit,''');') FROM ro_ticketing.PRODUCTS;
SELECT CONCAT('INSERT INTO ro_ticketing.PRODUCT (id,name,unitId,isIncludedInReport) VALUE (',id,',''',name,''',',unit,',1);') FROM ro_ticketing.PRODUCTS;