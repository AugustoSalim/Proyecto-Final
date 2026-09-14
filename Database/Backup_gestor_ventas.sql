--
-- PostgreSQL database dump
--

-- Dumped from database version 17.4
-- Dumped by pg_dump version 17.4

-- Started on 2026-09-14 00:16:15

SET statement_timeout = 0;
SET lock_timeout = 0;
SET idle_in_transaction_session_timeout = 0;
SET transaction_timeout = 0;
SET client_encoding = 'UTF8';
SET standard_conforming_strings = on;
SELECT pg_catalog.set_config('search_path', '', false);
SET check_function_bodies = false;
SET xmloption = content;
SET client_min_messages = warning;
SET row_security = off;

SET default_tablespace = '';

SET default_table_access_method = heap;

--
-- TOC entry 218 (class 1259 OID 33636)
-- Name: permiso; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.permiso (
    id_permiso integer NOT NULL,
    nombre character varying(100) NOT NULL,
    descripcion character varying(255)
);


ALTER TABLE public.permiso OWNER TO postgres;

--
-- TOC entry 217 (class 1259 OID 33635)
-- Name: permiso_id_permiso_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public.permiso_id_permiso_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE public.permiso_id_permiso_seq OWNER TO postgres;

--
-- TOC entry 4851 (class 0 OID 0)
-- Dependencies: 217
-- Name: permiso_id_permiso_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.permiso_id_permiso_seq OWNED BY public.permiso.id_permiso;


--
-- TOC entry 225 (class 1259 OID 33688)
-- Name: refresh_token; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.refresh_token (
    id_refresh_token integer NOT NULL,
    token character varying(255) NOT NULL,
    fecha_expiracion timestamp with time zone NOT NULL,
    revocado boolean DEFAULT false,
    id_usuario integer NOT NULL
);


ALTER TABLE public.refresh_token OWNER TO postgres;

--
-- TOC entry 224 (class 1259 OID 33687)
-- Name: refresh_token_id_refresh_token_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public.refresh_token_id_refresh_token_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE public.refresh_token_id_refresh_token_seq OWNER TO postgres;

--
-- TOC entry 4852 (class 0 OID 0)
-- Dependencies: 224
-- Name: refresh_token_id_refresh_token_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.refresh_token_id_refresh_token_seq OWNED BY public.refresh_token.id_refresh_token;


--
-- TOC entry 220 (class 1259 OID 33645)
-- Name: rol; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.rol (
    id_rol integer NOT NULL,
    nombre character varying(50) NOT NULL,
    es_personalizado boolean DEFAULT false
);


ALTER TABLE public.rol OWNER TO postgres;

--
-- TOC entry 219 (class 1259 OID 33644)
-- Name: rol_id_rol_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public.rol_id_rol_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE public.rol_id_rol_seq OWNER TO postgres;

--
-- TOC entry 4853 (class 0 OID 0)
-- Dependencies: 219
-- Name: rol_id_rol_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.rol_id_rol_seq OWNED BY public.rol.id_rol;


--
-- TOC entry 221 (class 1259 OID 33654)
-- Name: rol_permiso; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.rol_permiso (
    id_rol integer NOT NULL,
    id_permiso integer NOT NULL
);


ALTER TABLE public.rol_permiso OWNER TO postgres;

--
-- TOC entry 223 (class 1259 OID 33670)
-- Name: usuario; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.usuario (
    id_usuario integer NOT NULL,
    nombre_usuario character varying(100) NOT NULL,
    mail character varying(100) NOT NULL,
    password_hash character varying(255) NOT NULL,
    id_rol integer NOT NULL,
    activo boolean DEFAULT true,
    fecha_creacion timestamp with time zone DEFAULT CURRENT_TIMESTAMP
);


ALTER TABLE public.usuario OWNER TO postgres;

--
-- TOC entry 222 (class 1259 OID 33669)
-- Name: usuario_id_usuario_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public.usuario_id_usuario_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE public.usuario_id_usuario_seq OWNER TO postgres;

--
-- TOC entry 4854 (class 0 OID 0)
-- Dependencies: 222
-- Name: usuario_id_usuario_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.usuario_id_usuario_seq OWNED BY public.usuario.id_usuario;


--
-- TOC entry 4660 (class 2604 OID 33639)
-- Name: permiso id_permiso; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.permiso ALTER COLUMN id_permiso SET DEFAULT nextval('public.permiso_id_permiso_seq'::regclass);


--
-- TOC entry 4666 (class 2604 OID 33691)
-- Name: refresh_token id_refresh_token; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.refresh_token ALTER COLUMN id_refresh_token SET DEFAULT nextval('public.refresh_token_id_refresh_token_seq'::regclass);


--
-- TOC entry 4661 (class 2604 OID 33648)
-- Name: rol id_rol; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.rol ALTER COLUMN id_rol SET DEFAULT nextval('public.rol_id_rol_seq'::regclass);


--
-- TOC entry 4663 (class 2604 OID 33673)
-- Name: usuario id_usuario; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.usuario ALTER COLUMN id_usuario SET DEFAULT nextval('public.usuario_id_usuario_seq'::regclass);


--
-- TOC entry 4838 (class 0 OID 33636)
-- Dependencies: 218
-- Data for Name: permiso; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.permiso (id_permiso, nombre, descripcion) FROM stdin;
1	ventas_ver	Ver historial y listado de ventas
2	ventas_crear	Registrar una nueva venta
3	ventas_anular	Anular una venta
4	servicios_ver	Ver órdenes de trabajo
5	servicios_crear	Registrar una orden de trabajo
6	servicios_modificar	Cambiar datos de ordenes de trbaajo
7	usuarios_gestionar	Crear usuarios y administrar roles y permisos
\.


--
-- TOC entry 4845 (class 0 OID 33688)
-- Dependencies: 225
-- Data for Name: refresh_token; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.refresh_token (id_refresh_token, token, fecha_expiracion, revocado, id_usuario) FROM stdin;
\.


--
-- TOC entry 4840 (class 0 OID 33645)
-- Dependencies: 220
-- Data for Name: rol; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.rol (id_rol, nombre, es_personalizado) FROM stdin;
1	Administrador	f
2	Tecnico	f
3	Vendedor	f
\.


--
-- TOC entry 4841 (class 0 OID 33654)
-- Dependencies: 221
-- Data for Name: rol_permiso; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.rol_permiso (id_rol, id_permiso) FROM stdin;
1	1
1	2
1	3
1	4
1	5
1	6
1	7
2	4
2	5
2	6
\.


--
-- TOC entry 4843 (class 0 OID 33670)
-- Dependencies: 223
-- Data for Name: usuario; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.usuario (id_usuario, nombre_usuario, mail, password_hash, id_rol, activo, fecha_creacion) FROM stdin;
1	admin	augustosalim98@gmail.com	$2a$11$N9qo8uLOickgx2ZMRZoMyeIjZAgcfl7p92ldGxad68LJZdL17lhWy	1	t	2026-09-13 21:13:37.849894-03
\.


--
-- TOC entry 4855 (class 0 OID 0)
-- Dependencies: 217
-- Name: permiso_id_permiso_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.permiso_id_permiso_seq', 8, true);


--
-- TOC entry 4856 (class 0 OID 0)
-- Dependencies: 224
-- Name: refresh_token_id_refresh_token_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.refresh_token_id_refresh_token_seq', 1, false);


--
-- TOC entry 4857 (class 0 OID 0)
-- Dependencies: 219
-- Name: rol_id_rol_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.rol_id_rol_seq', 3, true);


--
-- TOC entry 4858 (class 0 OID 0)
-- Dependencies: 222
-- Name: usuario_id_usuario_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.usuario_id_usuario_seq', 1, true);


--
-- TOC entry 4669 (class 2606 OID 33643)
-- Name: permiso permiso_nombre_key; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.permiso
    ADD CONSTRAINT permiso_nombre_key UNIQUE (nombre);


--
-- TOC entry 4671 (class 2606 OID 33641)
-- Name: permiso permiso_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.permiso
    ADD CONSTRAINT permiso_pkey PRIMARY KEY (id_permiso);


--
-- TOC entry 4685 (class 2606 OID 33694)
-- Name: refresh_token refresh_token_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.refresh_token
    ADD CONSTRAINT refresh_token_pkey PRIMARY KEY (id_refresh_token);


--
-- TOC entry 4687 (class 2606 OID 33696)
-- Name: refresh_token refresh_token_token_key; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.refresh_token
    ADD CONSTRAINT refresh_token_token_key UNIQUE (token);


--
-- TOC entry 4673 (class 2606 OID 33653)
-- Name: rol rol_nombre_key; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.rol
    ADD CONSTRAINT rol_nombre_key UNIQUE (nombre);


--
-- TOC entry 4677 (class 2606 OID 33658)
-- Name: rol_permiso rol_permiso_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.rol_permiso
    ADD CONSTRAINT rol_permiso_pkey PRIMARY KEY (id_rol, id_permiso);


--
-- TOC entry 4675 (class 2606 OID 33651)
-- Name: rol rol_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.rol
    ADD CONSTRAINT rol_pkey PRIMARY KEY (id_rol);


--
-- TOC entry 4679 (class 2606 OID 33681)
-- Name: usuario usuario_mail_key; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.usuario
    ADD CONSTRAINT usuario_mail_key UNIQUE (mail);


--
-- TOC entry 4681 (class 2606 OID 33679)
-- Name: usuario usuario_nombre_usuario_key; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.usuario
    ADD CONSTRAINT usuario_nombre_usuario_key UNIQUE (nombre_usuario);


--
-- TOC entry 4683 (class 2606 OID 33677)
-- Name: usuario usuario_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.usuario
    ADD CONSTRAINT usuario_pkey PRIMARY KEY (id_usuario);


--
-- TOC entry 4688 (class 2606 OID 33664)
-- Name: rol_permiso fk_permiso; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.rol_permiso
    ADD CONSTRAINT fk_permiso FOREIGN KEY (id_permiso) REFERENCES public.permiso(id_permiso) ON DELETE CASCADE;


--
-- TOC entry 4691 (class 2606 OID 33697)
-- Name: refresh_token fk_refresh_usuario; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.refresh_token
    ADD CONSTRAINT fk_refresh_usuario FOREIGN KEY (id_usuario) REFERENCES public.usuario(id_usuario) ON DELETE CASCADE;


--
-- TOC entry 4689 (class 2606 OID 33659)
-- Name: rol_permiso fk_rol; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.rol_permiso
    ADD CONSTRAINT fk_rol FOREIGN KEY (id_rol) REFERENCES public.rol(id_rol) ON DELETE CASCADE;


--
-- TOC entry 4690 (class 2606 OID 33682)
-- Name: usuario fk_usuario_rol; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.usuario
    ADD CONSTRAINT fk_usuario_rol FOREIGN KEY (id_rol) REFERENCES public.rol(id_rol);


-- Completed on 2026-09-14 00:16:15

--
-- PostgreSQL database dump complete
--

