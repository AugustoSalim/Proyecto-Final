create table permiso(
	id_permiso serial primary key,
	nombre varchar(100) not null unique,
	descripcion varchar(255)
);

create table rol(
	id_rol serial PRIMARY key,
	nombre varchar(50) not null unique,
	es_personalizado boolean DEFAULT FALSE
);

create table rol_permiso(
	id_rol int not null,
	id_permiso int not null,
	primary key(id_rol, id_permiso),
	constraint fk_rol foreign key (id_rol) references rol(id_rol) on delete cascade,
	constraint fk_permiso foreign key (id_permiso) references permiso(id_permiso) on delete cascade
);

create table usuario(
	id_usuario serial primary key,
	nombre_usuario varchar(100) not null unique,
	mail varchar(100) not null unique,
	password_hash varchar(255) not null,
	id_rol int not null,
	activo boolean default true,
	fecha_creacion TIMESTAMPTZ DEFAULT current_timestamp,
	constraint fk_usuario_rol foreign key (id_rol) references rol(id_rol)
);

create table refresh_token(
	id_refresh_token serial primary key,
	token varchar(255) not null unique,
	fecha_expiracion timestamptz not null,
	revocado boolean default false,
	id_usuario int not null,
	constraint fk_refresh_usuario foreign key (id_usuario) references usuario(id_usuario) on delete cascade
);

-- Datos de carga
INSERT INTO permiso (nombre, descripcion) VALUES
    ('ventas_ver', 'Ver historial y listado de ventas'),
    ('ventas_crear', 'Registrar una nueva venta'),
    ('ventas_anular', 'Anular una venta'),
    ('servicios_ver', 'Ver órdenes de trabajo'),
    ('servicios_crear', 'Registrar una orden de trabajo'),
    ('servicios_modificar', 'Cambiar datos de ordenes de trbaajo'),
    ('usuarios_gestionar', 'Crear usuarios y administrar roles y permisos');

INSERT INTO rol (nombre, es_personalizado) VALUES
    ('Administrador', FALSE),
    ('Tecnico', FALSE),
    ('Vendedor', FALSE)
ON CONFLICT (nombre) DO NOTHING;

INSERT INTO rol_permiso (id_rol, id_permiso)
SELECT r.id_rol, p.id_permiso
FROM rol r 
CROSS JOIN permiso p
WHERE r.nombre = 'Administrador'
ON CONFLICT DO NOTHING;

INSERT INTO rol_permiso (id_rol, id_permiso)
SELECT r.id_rol, p.id_permiso
FROM rol r
JOIN permiso p ON p.nombre IN ('servicios_ver', 'servicios_crear', 'servicios_modificar')
WHERE r.nombre = 'Tecnico'
ON CONFLICT DO NOTHING;

INSERT INTO usuario (nombre_usuario, mail, password_hash, id_rol, activo)
VALUES (
    'admin',
    'augustosalim98@gmail.com',
    '$2a$11$N9qo8uLOickgx2ZMRZoMyeIjZAgcfl7p92ldGxad68LJZdL17lhWy',
    (SELECT id_rol FROM rol WHERE nombre = 'Administrador'),
    TRUE
)
ON CONFLICT (nombre_usuario) DO NOTHING;