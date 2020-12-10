CREATE TABLE public.category (
    id  integer NOT NULL primary key,
    name varchar(50) NOT NULL
);

CREATE TABLE public.supplier (
    id  integer NOT NULL primary key,
    name varchar(50) NOT NULL,
    contact_name varchar(255) NULL,
    contact_phone varchar(50) NULL,
    contact_email varchar(255) NULL
);

CREATE SEQUENCE product_id_seq;
CREATE TABLE public.product (
    id  integer NOT NULL default nextval('product_id_seq'),
    code varchar(50) NOT NULL,
    name varchar(50) NOT NULL,
    description varchar(250) NULL,
    cost money NULL,
    list_price money NULL,
    category_id integer NOT NULL,
    supplier_id integer NOT NULL,
    release_date timestamp ,
    created_on timestamp ,
    CONSTRAINT fk_category_id
      FOREIGN KEY(category_id) 
        REFERENCES category(id),
    CONSTRAINT fk_supplier_id
      FOREIGN KEY(supplier_id) 
        REFERENCES supplier(id)
);
