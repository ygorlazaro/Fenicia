-- ============================================================
-- FENICIA — SEED DE DADOS (massivo)
-- Schemas: auth, basic, project, social_network, public
--
-- AVISO: Este seed pressupõe que o banco já está com todas as
-- migrations do EF Core aplicadas (estrutura do snapshot atual).
-- Se o banco estiver desatualizado, execute:
--   dotnet ef database update
-- antes de rodar este script.
--
-- Quantidade por entidade (aprox.):
--   auth.states: 27
--   auth.companies: 1
--   auth.roles: 4
--   auth.users: 100
--   basic.positions: 100
--   basic.product_categories: 50
--   auth.modules: 12
--   auth.addresses: 100
--   basic.people: 200
--   basic.suppliers: 30
--   basic.customers: 50
--   basic.employees: 80
--   basic.products: 100
--   basic.orders: 80
--   basic.order_details: 200
--   basic.stock_movements: 100
--   auth.orders: 40
--   auth.order_details: 100
--   auth.subscriptions: 30
--   auth.subscription_credits: 60
--   auth.configuration: 100
--   auth.notifications: 80
--   auth.forgotten_passwords: 20
--   public.uploads: 100
--   social_network.profiles: 100
--   project.projects: 20
--   project.statuses: 60
--   project.sprints: 40
--   project.tasks: 100
--   project.project_subtasks: 200
--   project.task_assignees: 200
--   project.comments: 100
--   project.attachments: 50
--   project.teams: 20
--   project.team_users: 60
--   social_network.feeds: 200
--   social_network.comments: 300
--   social_network.likes: 400
--   social_network.shares: 50
--   social_network.friendships: 100
--   social_network.blocks: 10
--   social_network.reports: 20
--   social_network.attachments: 50
-- ============================================================

BEGIN;

-- UUID fixo da empresa seed

-- ============================================================
-- 1. ESTADOS (27 UFs do Brasil)
-- ============================================================
INSERT INTO auth.states (id, name, uf, created, updated)
SELECT
   md5('estado-' || i::text)::uuid,
   nome,
   uf,
   now() - (random() * interval '365 days'),
   now() - (random() * interval '365 days')
FROM (VALUES
   (1, 'Acre', 'AC'),
   (2, 'Alagoas', 'AL'),
   (3, 'Amapá', 'AP'),
   (4, 'Amazonas', 'AM'),
   (5, 'Bahia', 'BA'),
   (6, 'Ceará', 'CE'),
   (7, 'Distrito Federal', 'DF'),
   (8, 'Espírito Santo', 'ES'),
   (9, 'Goiás', 'GO'),
   (10, 'Maranhão', 'MA'),
   (11, 'Mato Grosso', 'MT'),
   (12, 'Mato Grosso do Sul', 'MS'),
   (13, 'Minas Gerais', 'MG'),
   (14, 'Pará', 'PA'),
   (15, 'Paraíba', 'PB'),
   (16, 'Paraná', 'PR'),
   (17, 'Pernambuco', 'PE'),
   (18, 'Piauí', 'PI'),
   (19, 'Rio de Janeiro', 'RJ'),
   (20, 'Rio Grande do Norte', 'RN'),
   (21, 'Rio Grande do Sul', 'RS'),
   (22, 'Rondônia', 'RO'),
   (23, 'Roraima', 'RR'),
   (24, 'Santa Catarina', 'SC'),
   (25, 'São Paulo', 'SP'),
   (26, 'Sergipe', 'SE'),
   (27, 'Tocantins', 'TO')
) AS t(i, nome, uf)
ON CONFLICT (id) DO NOTHING;

-- ============================================================
-- 2. EMPRESA
-- ============================================================
INSERT INTO auth.companies (id, name, cnpj, is_active, created, updated)
VALUES (
  '00000000-0000-0000-0000-000000000001'::uuid,
  'Gato Ninja',
  '23351185000184',
  true,
  now() - interval '365 days',
  now()
)
ON CONFLICT (id) DO NOTHING;

-- ============================================================
-- 3. ROLES (4 roles)
-- ============================================================
INSERT INTO auth.roles (id, name, created, updated)
SELECT
  md5('role-' || i::text)::uuid,
  CASE i
    WHEN 1 THEN 'admin'
    WHEN 2 THEN 'basic'
    WHEN 3 THEN 'project'
    WHEN 4 THEN 'social'
  END,
  now() - interval '365 days',
  now()
FROM generate_series(1, 4) AS i
ON CONFLICT (id) DO NOTHING;

-- ============================================================
-- 4. USUÁRIOS (100)
-- ============================================================
INSERT INTO auth.users (id, name, email, password, image_url, created, updated)
SELECT
  md5('user-' || i::text)::uuid,
  'Usuário ' || i,
  'user' || i || '@fenicia.com',
  md5('senha-' || i::text),
  CASE WHEN i % 5 = 0 THEN 'https://i.pravatar.cc/150?u=' || i ELSE NULL END,
  now() - (random() * interval '365 days'),
  now() - (random() * interval '30 days')
FROM generate_series(1, 100) AS i
ON CONFLICT (id) DO NOTHING;

-- ============================================================
-- 5. USERS_ROLES (100)
-- ============================================================
INSERT INTO auth.users_roles (id, user_id, role_id, company_id, created, updated)
SELECT
  md5('ur-' || i::text)::uuid,
  md5('user-' || i::text)::uuid,
  md5('role-' || ((i-1) % 4 + 1)::text)::uuid,
  '00000000-0000-0000-0000-000000000001'::uuid,
  now() - (random() * interval '365 days'),
  now() - (random() * interval '30 days')
FROM generate_series(1, 100) AS i
ON CONFLICT (id) DO NOTHING;

-- ============================================================
-- 6. CARGOS (100)
-- ============================================================
INSERT INTO basic.positions (id, company_id, name, created, updated)
SELECT
  md5('pos-' || i::text)::uuid,
  '00000000-0000-0000-0000-000000000001'::uuid,
  CASE i
    WHEN 1 THEN 'CEO'
    WHEN 2 THEN 'CTO'
    WHEN 3 THEN 'CFO'
    WHEN 4 THEN 'Diretor de Operações'
    WHEN 5 THEN 'Gerente de TI'
    WHEN 6 THEN 'Gerente de RH'
    WHEN 7 THEN 'Gerente de Vendas'
    WHEN 8 THEN 'Gerente de Marketing'
    WHEN 9 THEN 'Gerente de Projetos'
    WHEN 10 THEN 'Gerente Financeiro'
    WHEN 11 THEN 'Coordenador de Desenvolvimento'
    WHEN 12 THEN 'Coordenador de Suporte'
    WHEN 13 THEN 'Coordenador de Qualidade'
    WHEN 14 THEN 'Analista de Sistemas Sênior'
    WHEN 15 THEN 'Analista de Sistemas Pleno'
    WHEN 16 THEN 'Analista de Sistemas Júnior'
    WHEN 17 THEN 'Desenvolvedor Full Stack Sênior'
    WHEN 18 THEN 'Desenvolvedor Full Stack Pleno'
    WHEN 19 THEN 'Desenvolvedor Full Stack Júnior'
    WHEN 20 THEN 'Desenvolvedor Backend Sênior'
    WHEN 21 THEN 'Desenvolvedor Backend Pleno'
    WHEN 22 THEN 'Desenvolvedor Backend Júnior'
    WHEN 23 THEN 'Desenvolvedor Frontend Sênior'
    WHEN 24 THEN 'Desenvolvedor Frontend Pleno'
    WHEN 25 THEN 'Desenvolvedor Frontend Júnior'
    WHEN 26 THEN 'Desenvolvedor Mobile Sênior'
    WHEN 27 THEN 'Desenvolvedor Mobile Pleno'
    WHEN 28 THEN 'Desenvolvedor Mobile Júnior'
    WHEN 29 THEN 'Engenheiro de Dados'
    WHEN 30 THEN 'Cientista de Dados'
    WHEN 31 THEN 'Analista de Dados'
    WHEN 32 THEN 'DevOps Engineer Sênior'
    WHEN 33 THEN 'DevOps Engineer Pleno'
    WHEN 34 THEN 'DevOps Engineer Júnior'
    WHEN 35 THEN 'QA Engineer Sênior'
    WHEN 36 THEN 'QA Engineer Pleno'
    WHEN 37 THEN 'QA Engineer Júnior'
    WHEN 38 THEN 'Product Owner'
    WHEN 39 THEN 'Scrum Master'
    WHEN 40 THEN 'Tech Lead'
    WHEN 41 THEN 'Arquiteto de Software'
    WHEN 42 THEN 'Designer UX Sênior'
    WHEN 43 THEN 'Designer UX Pleno'
    WHEN 44 THEN 'Designer UX Júnior'
    WHEN 45 THEN 'Designer UI Sênior'
    WHEN 46 THEN 'Designer UI Pleno'
    WHEN 47 THEN 'Designer UI Júnior'
    WHEN 48 THEN 'Analista de Marketing Digital'
    WHEN 49 THEN 'Especialista em SEO'
    WHEN 50 THEN 'Social Media Manager'
    WHEN 51 THEN 'Redator Publicitário'
    WHEN 52 THEN 'Analista de Vendas'
    WHEN 53 THEN 'Executivo de Contas'
    WHEN 54 THEN 'Representante Comercial'
    WHEN 55 THEN 'Analista de Suporte Técnico'
    WHEN 56 THEN 'Especialista de Suporte N2'
    WHEN 57 THEN 'Especialista de Suporte N3'
    WHEN 58 THEN 'Analista de Infraestrutura'
    WHEN 59 THEN 'Administrador de Redes'
    WHEN 60 THEN 'Analista de Segurança da Informação'
    WHEN 61 THEN 'Especialista em Cloud'
    WHEN 62 THEN 'SRE Engineer'
    WHEN 63 THEN 'Analista de QA Manual'
    WHEN 64 THEN 'Analista de QA Automatizado'
    WHEN 65 THEN 'Product Manager'
    WHEN 66 THEN 'Business Analyst'
    WHEN 67 THEN 'Analista de BI'
    WHEN 68 THEN 'Analista de Compliance'
    WHEN 69 THEN 'Advogado Corporativo'
    WHEN 70 THEN 'Assistente Jurídico'
    WHEN 71 THEN 'Analista de Recrutamento e Seleção'
    WHEN 72 THEN 'Especialista em Treinamento'
    WHEN 73 THEN 'Analista de Folha de Pagamento'
    WHEN 74 THEN 'Assistente Administrativo'
    WHEN 75 THEN 'Recepcionista'
    WHEN 76 THEN 'Auxiliar de Escritório'
    WHEN 77 THEN 'Analista de Controladoria'
    WHEN 78 THEN 'Analista Fiscal'
    WHEN 79 THEN 'Analista Contábil'
    WHEN 80 THEN 'Tesoureiro'
    WHEN 81 THEN 'Analista de Crédito'
    WHEN 82 THEN 'Analista de Cobrança'
    WHEN 83 THEN 'Analista de Logística'
    WHEN 84 THEN 'Coordenador de Logística'
    WHEN 85 THEN 'Analista de Compras'
    WHEN 86 THEN 'Comprador Pleno'
    WHEN 87 THEN 'Comprador Sênior'
    WHEN 88 THEN 'Analista de Importação'
    WHEN 89 THEN 'Analista de Exportação'
    WHEN 90 THEN 'Analista de Qualidade'
    WHEN 91 THEN 'Inspetor de Qualidade'
    WHEN 92 THEN 'Analista de Meio Ambiente'
    WHEN 93 THEN 'Técnico de Segurança do Trabalho'
    WHEN 94 THEN 'Analista de Comunicação Interna'
    WHEN 95 THEN 'Assessor de Imprensa'
    WHEN 96 THEN 'Analista de Relações Públicas'
    WHEN 97 THEN 'Designer Gráfico'
    WHEN 98 THEN 'Editor de Vídeo'
    WHEN 99 THEN 'Fotógrafo'
    WHEN 100 THEN 'Videomaker'
  END,
  now() - (random() * interval '365 days'),
  now() - (random() * interval '30 days')
FROM generate_series(1, 100) AS i
ON CONFLICT (id) DO NOTHING;

-- ============================================================
-- 7. MÓDULOS (12)
-- ============================================================
INSERT INTO auth.modules (id, name, price, type, description, icon, is_active, sort_order, created, updated)
SELECT
  md5('mod-' || i::text)::uuid,
  CASE i
    WHEN 0 THEN 'Autenticação'
    WHEN 1 THEN 'Básico'
    WHEN 2 THEN 'Social Network'
    WHEN 3 THEN 'Projetos'
    WHEN 4 THEN 'Avaliação de Desempenho'
    WHEN 5 THEN 'Contabilidade'
    WHEN 6 THEN 'Recursos Humanos'
    WHEN 7 THEN 'PDV'
    WHEN 8 THEN 'Contratos'
    WHEN 9 THEN 'E-commerce'
    WHEN 10 THEN 'Atendimento ao Cliente'
    WHEN 11 THEN 'Plus'
  END,
  (random() * 500 + 50)::numeric(10,2),
  i,
  'Descrição do módulo ' || i,
  'icon-' || i,
  true,
  i,
  now() - interval '365 days',
  now()
FROM generate_series(0, 11) AS i
ON CONFLICT (id) DO NOTHING;

-- ============================================================
-- 8. ENDEREÇOS (100)
-- ============================================================
INSERT INTO auth.addresses (
  id, address_type, city, complement, country, created, deleted, is_default,
  latitude, longitude, neighborhood, number, observation, state_id, street, updated, zip_code
)
SELECT
  md5('addr-' || i::text)::uuid,
  (i % 4) + 1,
  cidade,
  CASE WHEN i % 3 = 0 THEN 'Apto ' || (i % 20 + 1) ELSE NULL END,
  'Brasil',
  now() - (random() * interval '365 days'),
  NULL,
  i % 10 = 0,
  -23.55 + (random() - 0.5) * 0.5,
  -46.63 + (random() - 0.5) * 0.5,
  'Bairro ' || ((i % 20) + 1),
  (i % 1000 + 1)::text,
  CASE WHEN i % 5 = 0 THEN 'Observação endereço ' || i ELSE NULL END,
  md5('estado-' || ((i % 27) + 1)::text)::uuid,
  'Rua ' || ((i % 50) + 1),
  now() - (random() * interval '30 days'),
  lpad((i % 90000000 + 10000000)::text, 8, '0')
FROM generate_series(1, 100) AS i
CROSS JOIN LATERAL (
  VALUES
    (1, 'São Paulo'),
    (2, 'Rio de Janeiro'),
    (3, 'Belo Horizonte'),
    (4, 'Curitiba'),
    (5, 'Porto Alegre'),
    (6, 'Salvador'),
    (7, 'Recife'),
    (8, 'Fortaleza'),
    (9, 'Brasília'),
    (10, 'Manaus'),
    (11, 'Belém'),
    (12, 'Goiânia'),
    (13, 'Campinas'),
    (14, 'São Luís'),
    (15, 'Maceió'),
    (16, 'Natal'),
    (17, 'Teresina'),
    (18, 'Cuiabá'),
    (19, 'Campo Grande'),
    (20, 'Florianópolis'),
    (21, 'João Pessoa'),
    (22, 'Aracaju'),
    (23, 'Palmas'),
    (24, 'Macapá'),
    (25, 'Boa Vista'),
    (26, 'Porto Velho'),
    (27, 'Rio Branco'),
    (28, 'Vitória'),
    (29, 'São Bernardo do Campo'),
    (30, 'Santo André'),
    (31, 'Osasco'),
    (32, 'Sorocaba'),
    (33, 'Ribeirão Preto'),
    (34, 'Uberlândia'),
    (35, 'Contagem'),
    (36, 'Juiz de Fora'),
    (37, 'Joinville'),
    (38, 'Londrina'),
    (39, 'Caxias do Sul'),
    (40, 'Maringá')
) AS cidades(seed, cidade)
WHERE i = cidades.seed OR (i % 40) + 1 = cidades.seed
ON CONFLICT (id) DO NOTHING;

-- Vincula endereço à empresa
UPDATE auth.companies SET address_id = md5('addr-1')::uuid WHERE id = '00000000-0000-0000-0000-000000000001'::uuid;

-- ============================================================
-- 9. PESSOAS (200)
-- ============================================================
INSERT INTO basic.people (
  id, company_id, name, document, email, phone_number, date_of_birth,
  photo_url, notes, state_model_id, created, updated
)
SELECT
  md5('person-' || i::text)::uuid,
  '00000000-0000-0000-0000-000000000001'::uuid,
  CASE WHEN i % 10 = 0 THEN 'Pessoa ' || i || ' Silva'
       WHEN i % 10 = 1 THEN 'Pessoa ' || i || ' Santos'
       WHEN i % 10 = 2 THEN 'Pessoa ' || i || ' Oliveira'
       WHEN i % 10 = 3 THEN 'Pessoa ' || i || ' Souza'
       WHEN i % 10 = 4 THEN 'Pessoa ' || i || ' Lima'
       WHEN i % 10 = 5 THEN 'Pessoa ' || i || ' Pereira'
       WHEN i % 10 = 6 THEN 'Pessoa ' || i || ' Costa'
       WHEN i % 10 = 7 THEN 'Pessoa ' || i || ' Rodrigues'
       WHEN i % 10 = 8 THEN 'Pessoa ' || i || ' Almeida'
       ELSE 'Pessoa ' || i || ' Ferreira'
  END,
  CASE WHEN i % 3 = 0 THEN lpad((i % 90000000000 + 10000000000)::text, 11, '0') ELSE NULL END,
  'pessoa' || i || '@email.com',
  '(' || ((i % 90) + 10) || ') 9' || lpad((i % 90000000 + 10000000)::text, 8, '0'),
  date '1980-01-01' + (i % 12000),
  CASE WHEN i % 4 = 0 THEN 'https://i.pravatar.cc/150?u=person-' || i ELSE NULL END,
  CASE WHEN i % 7 = 0 THEN 'Observações da pessoa ' || i ELSE NULL END,
  md5('estado-' || ((i % 27) + 1)::text)::uuid,
  now() - (random() * interval '365 days'),
  now() - (random() * interval '30 days')
FROM generate_series(1, 200) AS i
ON CONFLICT (id) DO NOTHING;

-- ============================================================
-- 10. CATEGORIAS DE PRODUTO (50)
-- ============================================================
INSERT INTO basic.product_categories (id, company_id, name, created, updated)
SELECT
  md5('cat-' || i::text)::uuid,
  '00000000-0000-0000-0000-000000000001'::uuid,
  CASE i
    WHEN 1 THEN 'Eletrônicos'
    WHEN 2 THEN 'Eletrodomésticos'
    WHEN 3 THEN 'Móveis'
    WHEN 4 THEN 'Informática'
    WHEN 5 THEN 'Periféricos'
    WHEN 6 THEN 'Celulares'
    WHEN 7 THEN 'Acessórios'
    WHEN 8 THEN 'Áudio e Vídeo'
    WHEN 9 THEN 'Games'
    WHEN 10 THEN 'Câmeras'
    WHEN 11 THEN 'Smart Home'
    WHEN 12 THEN 'Wearables'
    WHEN 13 THEN 'Papelaria'
    WHEN 14 THEN 'Escritório'
    WHEN 15 THEN 'Material Escolar'
    WHEN 16 THEN 'Limpeza'
    WHEN 17 THEN 'Higiene Pessoal'
    WHEN 18 THEN 'Alimentos'
    WHEN 19 THEN 'Bebidas'
    WHEN 20 THEN 'Congelados'
    WHEN 21 THEN 'Padaria'
    WHEN 22 THEN 'Hortifruti'
    WHEN 23 THEN 'Carnes'
    WHEN 24 THEN 'Laticínios'
    WHEN 25 THEN 'Bebidas Alcoólicas'
    WHEN 26 THEN 'Pet Shop'
    WHEN 27 THEN 'Farmacêutico'
    WHEN 28 THEN 'Beleza'
    WHEN 29 THEN 'Perfumaria'
    WHEN 30 THEN 'Automotivo'
    WHEN 31 THEN 'Ferramentas'
    WHEN 32 THEN 'Construção'
    WHEN 33 THEN 'Jardim'
    WHEN 34 THEN 'Esportes'
    WHEN 35 THEN 'Fitness'
    WHEN 36 THEN 'Camping'
    WHEN 37 THEN 'Brinquedos'
    WHEN 38 THEN 'Bebês'
    WHEN 39 THEN 'Moda Infantil'
    WHEN 40 THEN 'Moda Feminina'
    WHEN 41 THEN 'Moda Masculina'
    WHEN 42 THEN 'Calçados'
    WHEN 43 THEN 'Bolças e Mochilas'
    WHEN 44 THEN 'Relógios'
    WHEN 45 THEN 'Joias'
    WHEN 46 THEN 'Óculos'
    WHEN 47 THEN 'Livros'
    WHEN 48 THEN 'Revistas'
    WHEN 49 THEN 'Música'
    WHEN 50 THEN 'Filmes'
  END,
  now() - (random() * interval '365 days'),
  now() - (random() * interval '30 days')
FROM generate_series(1, 50) AS i
ON CONFLICT (id) DO NOTHING;

-- ============================================================
-- 11. FORNECEDORES (30)
-- ============================================================
INSERT INTO basic.suppliers (id, company_id, person_id, cnpj, created, updated)
SELECT
  md5('supp-' || i::text)::uuid,
  '00000000-0000-0000-0000-000000000001'::uuid,
  md5('person-' || (i + 50)::text)::uuid,
  lpad((i % 90000000000000 + 10000000000000)::text, 14, '0'),
  now() - (random() * interval '365 days'),
  now() - (random() * interval '30 days')
FROM generate_series(1, 30) AS i
ON CONFLICT (id) DO NOTHING;

-- ============================================================
-- 12. CLIENTES (50)
-- ============================================================
INSERT INTO basic.customers (id, company_id, person_id, created, updated)
SELECT
  md5('cust-' || i::text)::uuid,
  '00000000-0000-0000-0000-000000000001'::uuid,
  md5('person-' || (i + 100)::text)::uuid,
  now() - (random() * interval '365 days'),
  now() - (random() * interval '30 days')
FROM generate_series(1, 50) AS i
ON CONFLICT (id) DO NOTHING;

-- ============================================================
-- 13. FUNCIONÁRIOS (80)
-- ============================================================
INSERT INTO basic.employees (id, company_id, person_id, position_id, created, updated)
SELECT
  md5('emp-' || i::text)::uuid,
  '00000000-0000-0000-0000-000000000001'::uuid,
  md5('person-' || (i + 160)::text)::uuid,
  md5('pos-' || ((i % 100) + 1)::text)::uuid,
  now() - (random() * interval '365 days'),
  now() - (random() * interval '30 days')
FROM generate_series(1, 80) AS i
ON CONFLICT (id) DO NOTHING;

-- ============================================================
-- 14. PERSON_ADDRESSES (100)
-- ============================================================
INSERT INTO basic.person_addresses (id, company_id, person_id, address_id, created, updated)
SELECT
  md5('pa-' || i::text)::uuid,
  '00000000-0000-0000-0000-000000000001'::uuid,
  md5('person-' || ((i % 200) + 1)::text)::uuid,
  md5('addr-' || ((i % 100) + 1)::text)::uuid,
  now() - (random() * interval '365 days'),
  now() - (random() * interval '30 days')
FROM generate_series(1, 100) AS i
ON CONFLICT (id) DO NOTHING;

-- ============================================================
-- 15. PRODUTOS (100)
-- ============================================================
INSERT INTO basic.products (
  id, company_id, name, sku, barcode, description, cost_price, sales_price,
  quantity, min_stock_level, max_stock_level, image_url, weight, dimensions,
  unit_of_measure, category_id, supplier_id, is_active, created, updated
)
SELECT
  md5('prod-' || i::text)::uuid,
  '00000000-0000-0000-0000-000000000001'::uuid,
  'Produto ' || i || ' — ' || CASE (i % 10)
    WHEN 0 THEN 'Smartphone'
    WHEN 1 THEN 'Notebook'
    WHEN 2 THEN 'Monitor'
    WHEN 3 THEN 'Teclado'
    WHEN 4 THEN 'Mouse'
    WHEN 5 THEN 'Cadeira'
    WHEN 6 THEN 'Mesa'
    WHEN 7 THEN 'Impressora'
    WHEN 8 THEN 'Headset'
    ELSE 'Webcam'
  END,
  'SKU-' || lpad(i::text, 5, '0'),
  lpad((i % 9000000000000 + 1000000000000)::text, 13, '0'),
  'Descrição detalhada do produto ' || i || ' com características técnicas.',
  (random() * 500 + 50)::numeric(10,2),
  (random() * 1000 + 100)::numeric(10,2),
  (random() * 500)::double precision,
  10 + (i % 40),
  200 + (i % 500),
  CASE WHEN i % 3 = 0 THEN 'https://picsum.photos/seed/prod-' || i || '/200/200' ELSE NULL END,
  (random() * 10 + 0.1)::numeric(10,2),
  (random() * 100 + 10)::text || 'x' || (random() * 100 + 10)::text || 'x' || (random() * 100 + 10)::text || ' cm',
  CASE WHEN i % 2 = 0 THEN 'UN' ELSE 'CX' END,
  md5('cat-' || ((i % 50) + 1)::text)::uuid,
  CASE WHEN i % 3 = 0 THEN md5('supp-' || ((i % 30) + 1)::text)::uuid ELSE NULL END,
  true,
  now() - (random() * interval '365 days'),
  now() - (random() * interval '30 days')
FROM generate_series(1, 100) AS i
ON CONFLICT (id) DO NOTHING;

-- ============================================================
-- 16. PEDIDOS BASIC (80)
-- ============================================================
INSERT INTO basic.orders (
  id, company_id, order_number, customer_id, employee_id, user_id,
  total_amount, discount_amount, total_quantity, sale_date, status,
  payment_method, notes, created, updated
)
SELECT
  md5('ord-' || i::text)::uuid,
  '00000000-0000-0000-0000-000000000001'::uuid,
  'PED-' || to_char(now() - (random() * interval '365 days'), 'YYYY') || '-' || lpad(i::text, 5, '0'),
  md5('cust-' || ((i % 50) + 1)::text)::uuid,
  md5('emp-' || ((i % 80) + 1)::text)::uuid,
  md5('user-' || ((i % 100) + 1)::text)::uuid,
  (random() * 5000 + 100)::numeric(10,2),
  (random() * 200)::numeric(10,2),
  (random() * 20 + 1)::int,
  now() - (random() * interval '365 days'),
  (i % 3)::int,
  (i % 6)::int,
  CASE WHEN i % 5 = 0 THEN 'Observação do pedido ' || i ELSE NULL END,
  now() - (random() * interval '365 days'),
  now() - (random() * interval '30 days')
FROM generate_series(1, 80) AS i
ON CONFLICT (id) DO NOTHING;

-- ============================================================
-- 17. ITENS DE PEDIDO BASIC (200)
-- ============================================================
INSERT INTO basic.order_details (
  id, company_id, order_id, product_id, price, discount_amount, subtotal, quantity, created, updated
)
SELECT
  md5('od-' || i::text)::uuid,
  '00000000-0000-0000-0000-000000000001'::uuid,
  md5('ord-' || ((i % 80) + 1)::text)::uuid,
  md5('prod-' || ((i % 100) + 1)::text)::uuid,
  (random() * 500 + 50)::numeric(10,2),
  (random() * 50)::numeric(10,2),
  (random() * 500 + 50)::numeric(10,2),
  (random() * 5 + 1)::double precision,
  now() - (random() * interval '365 days'),
  now() - (random() * interval '30 days')
FROM generate_series(1, 200) AS i
ON CONFLICT (id) DO NOTHING;

-- ============================================================
-- 18. MOVIMENTAÇÕES DE ESTOQUE (100)
-- ============================================================
INSERT INTO basic.stock_movements (
  id, company_id, product_id, quantity, date, price, type, reason,
  customer_id, supplier_id, employee_id, order_id, created, updated
)
SELECT
  md5('sm-' || i::text)::uuid,
  '00000000-0000-0000-0000-000000000001'::uuid,
  md5('prod-' || ((i % 100) + 1)::text)::uuid,
  (random() * 100 + 1)::double precision,
  now() - (random() * interval '365 days'),
  (random() * 500 + 50)::numeric(10,2),
  CASE WHEN i % 2 = 0 THEN 1 ELSE 2 END,
  CASE i % 4
    WHEN 0 THEN 'Compra de fornecedor'
    WHEN 1 THEN 'Venda para cliente'
    WHEN 2 THEN 'Ajuste de inventário'
    ELSE 'Devolução'
  END,
  CASE WHEN i % 3 = 0 THEN md5('cust-' || ((i % 50) + 1)::text)::uuid ELSE NULL END,
  CASE WHEN i % 3 = 0 THEN md5('supp-' || ((i % 30) + 1)::text)::uuid ELSE NULL END,
  CASE WHEN i % 3 = 0 THEN md5('emp-' || ((i % 80) + 1)::text)::uuid ELSE NULL END,
  CASE WHEN i % 3 = 0 THEN md5('ord-' || ((i % 80) + 1)::text)::uuid ELSE NULL END,
  now() - (random() * interval '365 days'),
  now() - (random() * interval '30 days')
FROM generate_series(1, 100) AS i
ON CONFLICT (id) DO NOTHING;

-- ============================================================
-- 19. PEDIDOS AUTH (40)
-- ============================================================
INSERT INTO auth.orders (
  id, company_id, order_number, user_id, total_amount, discount_amount,
  total_quantity, sale_date, status, payment_method, notes, created, updated
)
SELECT
  md5('authord-' || i::text)::uuid,
  '00000000-0000-0000-0000-000000000001'::uuid,
  'AUTH-' || to_char(now() - (random() * interval '365 days'), 'YYYY') || '-' || lpad(i::text, 5, '0'),
  md5('user-' || ((i % 100) + 1)::text)::uuid,
  (random() * 10000 + 500)::numeric(10,2),
  (random() * 500)::numeric(10,2),
  (random() * 10 + 1)::int,
  now() - (random() * interval '365 days'),
  (i % 3)::int,
  (i % 6)::int,
  CASE WHEN i % 5 = 0 THEN 'Pedido módulo ' || i ELSE NULL END,
  now() - (random() * interval '365 days'),
  now() - (random() * interval '30 days')
FROM generate_series(1, 40) AS i
ON CONFLICT (id) DO NOTHING;

-- ============================================================
-- 20. DETALHES DE PEDIDO AUTH (100)
-- ============================================================
INSERT INTO auth.order_details (
  id, order_id, module_id, price, discount_amount, subtotal, created, updated
)
SELECT
  md5('authod-' || i::text)::uuid,
  md5('authord-' || ((i % 40) + 1)::text)::uuid,
  md5('mod-' || ((i % 12))::text)::uuid,
  (random() * 500 + 50)::numeric(10,2),
  (random() * 50)::numeric(10,2),
  (random() * 500 + 50)::numeric(10,2),
  now() - (random() * interval '365 days'),
  now() - (random() * interval '30 days')
FROM generate_series(1, 100) AS i
ON CONFLICT (id) DO NOTHING;

-- ============================================================
-- 21. SUBSCRIPTIONS (30)
-- ============================================================
INSERT INTO auth.subscriptions (
  id, company_id, status, start_date, end_date, order_id, created, updated
)
SELECT
  md5('sub-' || i::text)::uuid,
  '00000000-0000-0000-0000-000000000001'::uuid,
  CASE WHEN i % 4 = 0 THEN 0 ELSE 1 END,
  now() - (random() * interval '365 days'),
  now() + (random() * interval '365 days'),
  md5('authord-' || ((i % 40) + 1)::text)::uuid,
  now() - (random() * interval '365 days'),
  now() - (random() * interval '30 days')
FROM generate_series(1, 30) AS i
ON CONFLICT (id) DO NOTHING;

-- ============================================================
-- 22. SUBSCRIPTION CREDITS (60)
-- ============================================================
INSERT INTO auth.subscription_credits (
  id, subscription_id, module_id, is_active, start_date, end_date,
  order_detail_id, created, updated
)
SELECT
  md5('cred-' || i::text)::uuid,
  md5('sub-' || ((i % 30) + 1)::text)::uuid,
  md5('mod-' || ((i % 12))::text)::uuid,
  i % 2 = 0,
  now() - (random() * interval '365 days'),
  now() + (random() * interval '365 days'),
  md5('authod-' || ((i % 100) + 1)::text)::uuid,
  now() - (random() * interval '365 days'),
  now() - (random() * interval '30 days')
FROM generate_series(1, 60) AS i
ON CONFLICT (id) DO NOTHING;

-- ============================================================
-- 23. CONFIGURAÇÕES (100)
-- ============================================================
INSERT INTO auth.configuration (
  id, company_id, config_type, value, user_id, created, updated
)
SELECT
  md5('conf-' || i::text)::uuid,
  '00000000-0000-0000-0000-000000000001'::uuid,
  i % 2,
  CASE i % 2
    WHEN 0 THEN 'pt-BR'
    ELSE 'America/Sao_Paulo'
  END,
  md5('user-' || ((i % 100) + 1)::text)::uuid,
  now() - (random() * interval '365 days'),
  now() - (random() * interval '30 days')
FROM generate_series(1, 100) AS i
ON CONFLICT (id) DO NOTHING;

-- ============================================================
-- 24. NOTIFICAÇÕES (80)
-- ============================================================
INSERT INTO auth.notifications (
  id, company_id, title, description, date, image_url, "read", created, updated
)
SELECT
  md5('notif-' || i::text)::uuid,
  '00000000-0000-0000-0000-000000000001'::uuid,
  'Notificação ' || i,
  'Descrição da notificação ' || i,
  now() - (random() * interval '365 days'),
  CASE WHEN i % 3 = 0 THEN 'https://picsum.photos/seed/notif-' || i || '/100/100' ELSE NULL END,
  i % 3 = 0,
  now() - (random() * interval '365 days'),
  now() - (random() * interval '30 days')
FROM generate_series(1, 80) AS i
ON CONFLICT (id) DO NOTHING;

-- ============================================================
-- 25. FORGOT PASSWORDS (20)
-- ============================================================
INSERT INTO auth.forgotten_passwords (
  id, user_id, code, expiration_date, is_active, ip_address, user_agent, created, updated
)
SELECT
  md5('fp-' || i::text)::uuid,
  md5('user-' || ((i % 100) + 1)::text)::uuid,
  md5('code-' || i::text)::text,
  now() + (random() * interval '7 days'),
  i % 2 = 0,
  '192.168.' || (i % 255 + 1) || '.' || (i % 255 + 1),
  'Mozilla/5.0 (Windows NT 10.0; Win64; x64)',
  now() - (random() * interval '7 days'),
  now() - (random() * interval '7 days')
FROM generate_series(1, 20) AS i
ON CONFLICT (id) DO NOTHING;

-- ============================================================
-- 26. UPLOADS (100) — schema public
-- ============================================================
INSERT INTO auth.uploads (
  id, original_file_name, stored_file_name, content_type, size_bytes, url, created, updated
)
SELECT
  md5('upl-' || i::text)::uuid,
  'arquivo-' || i || '.jpg',
  'stored-' || i || '.jpg',
  'image/jpeg',
  (random() * 500000 + 50000)::bigint,
  'https://fenicia.s3.amazonaws.com/uploads/stored-' || i || '.jpg',
  now() - (random() * interval '365 days'),
  now() - (random() * interval '30 days')
FROM generate_series(1, 100) AS i
ON CONFLICT (id) DO NOTHING;

-- ============================================================
-- 27. PROFILES (100) — social_network
-- ============================================================
INSERT INTO social_network.profiles (
  id, user_id, user_name, bio, upload_id, website, location, phone, birth_date, created, updated
)
SELECT
  md5('prof-' || i::text)::uuid,
  md5('user-' || i::text)::uuid,
  'usuario' || i,
  CASE WHEN i % 3 = 0 THEN 'Bio do usuário ' || i || ' apaixonado por tecnologia e inovação.' ELSE NULL END,
  md5('upl-' || ((i % 100) + 1)::text)::uuid,
  CASE WHEN i % 4 = 0 THEN 'https://usuario' || i || '.dev' ELSE NULL END,
  CASE WHEN i % 2 = 0 THEN 'São Paulo, SP' ELSE 'Rio de Janeiro, RJ' END,
  '(' || ((i % 90) + 10) || ') 9' || lpad((i % 90000000 + 10000000)::text, 8, '0'),
  date '1990-01-01' + (i % 12000),
  now() - (random() * interval '365 days'),
  now() - (random() * interval '30 days')
FROM generate_series(1, 100) AS i
ON CONFLICT (id) DO NOTHING;

-- ============================================================
-- 28. PROJECTS (20)
-- ============================================================
INSERT INTO project.projects (
  id, company_id, title, description, status, start_date, end_date, owner, created, updated
)
SELECT
  md5('proj-' || i::text)::uuid,
  '00000000-0000-0000-0000-000000000001'::uuid,
  'Projeto ' || i || ' — ' || CASE (i % 5)
    WHEN 0 THEN 'Plataforma E-commerce'
    WHEN 1 THEN 'App Mobile'
    WHEN 2 THEN 'Sistema de CRM'
    WHEN 3 THEN 'Portal do Cliente'
    ELSE 'Integração API'
  END,
  'Descrição do projeto ' || i || ' com escopo, objetivos e entregas definidas.',
  (i % 4)::int,
  now() - (random() * interval '365 days'),
  now() + (random() * interval '180 days'),
  md5('user-' || ((i % 100) + 1)::text)::uuid,
  now() - (random() * interval '365 days'),
  now() - (random() * interval '30 days')
FROM generate_series(1, 20) AS i
ON CONFLICT (id) DO NOTHING;

-- ============================================================
-- 29. PROJECT STATUSES (60)
-- ============================================================
INSERT INTO project.statuses (
  id, company_id, project_id, name, color, "order", is_final, created, updated
)
SELECT
  md5('pstat-' || i::text)::uuid,
  '00000000-0000-0000-0000-000000000001'::uuid,
  md5('proj-' || ((i % 20) + 1)::text)::uuid,
  CASE ((i-1) % 6)
    WHEN 0 THEN 'Backlog'
    WHEN 1 THEN 'A Fazer'
    WHEN 2 THEN 'Em Progresso'
    WHEN 3 THEN 'Em Revisão'
    WHEN 4 THEN 'Concluído'
    ELSE 'Bloqueado'
  END,
  CASE ((i-1) % 6)
    WHEN 0 THEN '#6c757d'
    WHEN 1 THEN '#0dcaf0'
    WHEN 2 THEN '#ffc107'
    WHEN 3 THEN '#fd7e14'
    WHEN 4 THEN '#198754'
    ELSE '#dc3545'
  END,
  ((i-1) % 6) + 1,
  ((i-1) % 6) = 4,
  now() - (random() * interval '365 days'),
  now() - (random() * interval '30 days')
FROM generate_series(1, 60) AS i
ON CONFLICT (id) DO NOTHING;

-- ============================================================
-- 30. SPRINTS (40)
-- ============================================================
INSERT INTO project.sprints (
  id, company_id, project_id, name, description, start_date, end_date, created_by, created, updated
)
SELECT
  md5('spr-' || i::text)::uuid,
  '00000000-0000-0000-0000-000000000001'::uuid,
  md5('proj-' || ((i % 20) + 1)::text)::uuid,
  'Sprint ' || i,
  'Sprint ' || i || ' do projeto ' || ((i % 20) + 1),
  now() - (random() * interval '180 days'),
  now() + (random() * interval '180 days'),
  md5('user-' || ((i % 100) + 1)::text)::uuid,
  now() - (random() * interval '180 days'),
  now() - (random() * interval '30 days')
FROM generate_series(1, 40) AS i
ON CONFLICT (id) DO NOTHING;

-- ============================================================
-- 31. TASKS (100)
-- ============================================================
INSERT INTO project.tasks (
  id, company_id, project_id, status_id, title, description, priority, type,
  "order", estimate_points, due_date, created_by, sprint_id, created, updated
)
SELECT
  md5('task-' || i::text)::uuid,
  '00000000-0000-0000-0000-000000000001'::uuid,
  md5('proj-' || ((i % 20) + 1)::text)::uuid,
  md5('pstat-' || ((i % 60) + 1)::text)::uuid,
  'Task ' || i || ' — ' || CASE (i % 5)
    WHEN 0 THEN 'Implementar login'
    WHEN 1 THEN 'Corrigir bug no checkout'
    WHEN 2 THEN 'Criar dashboard'
    WHEN 3 THEN 'Otimizar query'
    ELSE 'Refatorar módulo'
  END,
  'Descrição da task ' || i || ' com detalhes de implementação.',
  (i % 4)::int,
  (i % 5)::int,
  i,
  (i % 13 + 1),
  now() + (random() * interval '90 days'),
  md5('user-' || ((i % 100) + 1)::text)::uuid,
  CASE WHEN i % 2 = 0 THEN md5('spr-' || ((i % 40) + 1)::text)::uuid ELSE NULL END,
  now() - (random() * interval '365 days'),
  now() - (random() * interval '30 days')
FROM generate_series(1, 100) AS i
ON CONFLICT (id) DO NOTHING;

-- ============================================================
-- 32. SUBTASKS (200)
-- ============================================================
INSERT INTO project.project_subtasks (
  id, company_id, task_id, title, "order", is_completed, due_date, completed_at, created, updated
)
SELECT
  md5('subtask-' || i::text)::uuid,
  '00000000-0000-0000-0000-000000000001'::uuid,
  md5('task-' || ((i % 100) + 1)::text)::uuid,
  'Subtask ' || i || ' de ' || ((i % 100) + 1),
  i % 5 + 1,
  i % 3 = 0,
  now() + (random() * interval '60 days'),
  CASE WHEN i % 3 = 0 THEN now() - (random() * interval '30 days') ELSE NULL END,
  now() - (random() * interval '365 days'),
  now() - (random() * interval '30 days')
FROM generate_series(1, 200) AS i
ON CONFLICT (id) DO NOTHING;

-- ============================================================
-- 33. TASK ASSIGNEES (200)
-- ============================================================
INSERT INTO project.task_assignees (
  id, company_id, task_id, user_id, role, assigned_at, created, updated
)
SELECT
  md5('ta-' || i::text)::uuid,
  '00000000-0000-0000-0000-000000000001'::uuid,
  md5('task-' || ((i % 100) + 1)::text)::uuid,
  md5('user-' || ((i % 100) + 1)::text)::uuid,
  (i % 3)::int,
  now() - (random() * interval '365 days'),
  now() - (random() * interval '30 days'),
  now() - (random() * interval '30 days')
FROM generate_series(1, 200) AS i
ON CONFLICT (id) DO NOTHING;

-- ============================================================
-- 34. PROJECT COMMENTS (100)
-- ============================================================
INSERT INTO project.comments (
  id, company_id, task_id, user_id, author_id, content, created, updated
)
SELECT
  md5('pcom-' || i::text)::uuid,
  '00000000-0000-0000-0000-000000000001'::uuid,
  md5('task-' || ((i % 100) + 1)::text)::uuid,
  md5('user-' || ((i % 100) + 1)::text)::uuid,
  md5('user-' || ((i % 100) + 1)::text)::uuid,
  'Comentário no task ' || ((i % 100) + 1) || ' pelo usuário ' || ((i % 100) + 1) || '.',
  now() - (random() * interval '365 days'),
  now() - (random() * interval '30 days')
FROM generate_series(1, 100) AS i
ON CONFLICT (id) DO NOTHING;

-- ============================================================
-- 35. PROJECT ATTACHMENTS (50)
-- ============================================================
INSERT INTO project.attachments (
  id, company_id, task_id, file_name, file_url, file_size, size, content_type,
  uploaded_by, created, updated
)
SELECT
  md5('patt-' || i::text)::uuid,
  '00000000-0000-0000-0000-000000000001'::uuid,
  md5('task-' || ((i % 100) + 1)::text)::uuid,
  'anexo-' || i || '.pdf',
  'https://fenicia.s3.amazonaws.com/anexos/anexo-' || i || '.pdf',
  (random() * 5000000 + 500000)::bigint,
  (random() * 5000000 + 500000)::bigint,
  'application/pdf',
  md5('user-' || ((i % 100) + 1)::text)::uuid,
  now() - (random() * interval '365 days'),
  now() - (random() * interval '30 days')
FROM generate_series(1, 50) AS i
ON CONFLICT (id) DO NOTHING;

-- ============================================================
-- 36. TEAMS (20)
-- ============================================================
INSERT INTO project.teams (
  id, company_id, project_id, name, description, color, created_by, created, updated
)
SELECT
  md5('team-' || i::text)::uuid,
  '00000000-0000-0000-0000-000000000001'::uuid,
  md5('proj-' || ((i % 20) + 1)::text)::uuid,
  'Time ' || i || ' — ' || CASE (i % 5)
    WHEN 0 THEN 'Frontend'
    WHEN 1 THEN 'Backend'
    WHEN 2 THEN 'Mobile'
    WHEN 3 THEN 'QA'
    ELSE 'DevOps'
  END,
  'Time responsável pela squad ' || i,
  CASE (i % 5)
    WHEN 0 THEN '#0d6efd'
    WHEN 1 THEN '#198754'
    WHEN 2 THEN '#ffc107'
    WHEN 3 THEN '#dc3545'
    ELSE '#6f42c1'
  END,
  md5('user-' || ((i % 100) + 1)::text)::uuid,
  now() - (random() * interval '365 days'),
  now() - (random() * interval '30 days')
FROM generate_series(1, 20) AS i
ON CONFLICT (id) DO NOTHING;

-- ============================================================
-- 37. TEAM USERS (60)
-- ============================================================
INSERT INTO project.team_users (
  id, company_id, team_id, user_id, role, joined_at, created, updated
)
SELECT
  md5('tu-' || i::text)::uuid,
  '00000000-0000-0000-0000-000000000001'::uuid,
  md5('team-' || ((i % 20) + 1)::text)::uuid,
  md5('user-' || ((i % 100) + 1)::text)::uuid,
  (i % 2)::int,
  now() - (random() * interval '365 days'),
  now() - (random() * interval '30 days'),
  now() - (random() * interval '30 days')
FROM generate_series(1, 60) AS i
ON CONFLICT (id) DO NOTHING;

-- ============================================================
-- 38. FEEDS (200)
-- ============================================================
INSERT INTO social_network.feeds (
  id, company_id, profile_id, text, date, original_feed_id,
  total_likes, total_comments, total_shares, created, updated
)
SELECT
  md5('feed-' || i::text)::uuid,
  '00000000-0000-0000-0000-000000000001'::uuid,
  md5('prof-' || ((i % 100) + 1)::text)::uuid,
  'Post ' || i || ' — ' || CASE (i % 5)
    WHEN 0 THEN 'Aprendendo novas tecnologias! 🚀'
    WHEN 1 THEN 'Finalizando sprint com sucesso! ✅'
    WHEN 2 THEN 'Novo projeto na área! 🎉'
    WHEN 3 THEN 'Reflexão sobre agile... 🤔'
    ELSE 'Compartilhando conhecimento! 📚'
  END,
  now() - (random() * interval '365 days'),
  CASE WHEN i % 10 = 0 THEN md5('feed-' || ((i % 200) + 1)::text)::uuid ELSE NULL END,
  (random() * 100)::int,
  (random() * 30)::int,
  (random() * 20)::int,
  now() - (random() * interval '365 days'),
  now() - (random() * interval '30 days')
FROM generate_series(1, 200) AS i
ON CONFLICT (id) DO NOTHING;

-- ============================================================
-- 39. SOCIAL COMMENTS (300)
-- ============================================================
INSERT INTO social_network.comments (
  id, company_id, profile_id, feed_id, parent_comment_id, text,
  comment_date, updated_date, created, updated
)
SELECT
  md5('scom-' || i::text)::uuid,
  '00000000-0000-0000-0000-000000000001'::uuid,
  md5('prof-' || ((i % 100) + 1)::text)::uuid,
  md5('feed-' || ((i % 200) + 1)::text)::uuid,
  CASE WHEN i % 5 = 0 THEN md5('scom-' || ((i % 300) + 1)::text)::uuid ELSE NULL END,
  'Comentário ' || i || ' — ' || CASE (i % 3)
    WHEN 0 THEN 'Concordo totalmente!'
    WHEN 1 THEN 'Interessante ponto de vista.'
    ELSE 'Obrigado por compartilhar!'
  END,
  now() - (random() * interval '365 days'),
  CASE WHEN i % 4 = 0 THEN now() - (random() * interval '30 days') ELSE NULL END,
  now() - (random() * interval '365 days'),
  now() - (random() * interval '30 days')
FROM generate_series(1, 300) AS i
ON CONFLICT (id) DO NOTHING;

-- ============================================================
-- 40. LIKES (400)
-- ============================================================
INSERT INTO social_network.likes (
  id, company_id, profile_id, feed_id, comment_model_id, like_date, created, updated
)
SELECT
  md5('like-' || i::text)::uuid,
  '00000000-0000-0000-0000-000000000001'::uuid,
  md5('prof-' || ((i % 100) + 1)::text)::uuid,
  md5('feed-' || ((i % 200) + 1)::text)::uuid,
  CASE WHEN i % 3 = 0 THEN md5('scom-' || ((i % 300) + 1)::text)::uuid ELSE NULL END,
  now() - (random() * interval '365 days'),
  now() - (random() * interval '30 days'),
  now() - (random() * interval '30 days')
FROM generate_series(1, 400) AS i
ON CONFLICT (id) DO NOTHING;

-- ============================================================
-- 41. SHARES (50)
-- ============================================================
INSERT INTO social_network.shares (
  id, company_id, profile_id, original_feed_id, text, share_date, created, updated
)
SELECT
  md5('share-' || i::text)::uuid,
  '00000000-0000-0000-0000-000000000001'::uuid,
  md5('prof-' || ((i % 100) + 1)::text)::uuid,
  md5('feed-' || ((i % 200) + 1)::text)::uuid,
  CASE WHEN i % 2 = 0 THEN 'Compartilhando com meu comentário...' ELSE NULL END,
  now() - (random() * interval '365 days'),
  now() - (random() * interval '30 days'),
  now() - (random() * interval '30 days')
FROM generate_series(1, 50) AS i
ON CONFLICT (id) DO NOTHING;

-- ============================================================
-- 42. FRIENDSHIPS (100)
-- ============================================================
INSERT INTO social_network.friendships (
  id, profile_id, target_profile_id, follow_date, is_active, created, updated
)
SELECT
  md5('friend-' || i::text)::uuid,
  md5('prof-' || ((i % 100) + 1)::text)::uuid,
  md5('prof-' || (((i % 100) + 1) % 100 + 1)::text)::uuid,
  now() - (random() * interval '365 days'),
  i % 10 <> 0,
  now() - (random() * interval '365 days'),
  now() - (random() * interval '30 days')
FROM generate_series(1, 100) AS i
ON CONFLICT (id) DO NOTHING;

-- ============================================================
-- 43. BLOCKS (10)
-- ============================================================
INSERT INTO social_network.blocks (
  id, profile_id, blocked_profile_id, reason, block_date, is_active, created, updated
)
SELECT
  md5('block-' || i::text)::uuid,
  md5('prof-' || ((i % 100) + 1)::text)::uuid,
  md5('prof-' || (((i % 100) + 1) % 100 + 1)::text)::uuid,
  CASE i % 3
    WHEN 0 THEN 'Conteúdo impróprio'
    WHEN 1 THEN 'Spam'
    ELSE 'Assédio'
  END,
  now() - (random() * interval '180 days'),
  i % 2 = 0,
  now() - (random() * interval '180 days'),
  now() - (random() * interval '30 days')
FROM generate_series(1, 10) AS i
ON CONFLICT (id) DO NOTHING;

-- ============================================================
-- 44. REPORTS (20)
-- ============================================================
INSERT INTO social_network.reports (
  id, reporter_id, target_id, target_type, reason, description, status, report_date, created, updated
)
SELECT
  md5('rep-' || i::text)::uuid,
  md5('user-' || ((i % 100) + 1)::text)::uuid,
  md5('prof-' || ((i % 100) + 1)::text)::uuid,
  CASE i % 2 WHEN 0 THEN 'profile' ELSE 'feed' END,
  CASE i % 3
    WHEN 0 THEN 'Spam'
    WHEN 1 THEN 'Conteúdo ofensivo'
    ELSE 'Assédio'
  END,
  'Descrição detalhada da denúncia ' || i,
  (i % 3)::int,
  now() - (random() * interval '180 days'),
  now() - (random() * interval '180 days'),
  now() - (random() * interval '30 days')
FROM generate_series(1, 20) AS i
ON CONFLICT (id) DO NOTHING;

-- ============================================================
-- 45. SOCIAL ATTACHMENTS (50)
-- ============================================================
INSERT INTO social_network.attachments (
  id, company_id, comment_id, url, file_type, file_size, upload_date, created, updated
)
SELECT
  md5('satt-' || i::text)::uuid,
  '00000000-0000-0000-0000-000000000001'::uuid,
  md5('scom-' || ((i % 300) + 1)::text)::uuid,
  'https://fenicia.s3.amazonaws.com/social/attachment-' || i || '.jpg',
  'image/jpeg',
  (random() * 2000000 + 100000)::bigint,
  now() - (random() * interval '365 days'),
  now() - (random() * interval '30 days'),
  now() - (random() * interval '30 days')
FROM generate_series(1, 50) AS i
ON CONFLICT (id) DO NOTHING;

-- ============================================================
-- VALIDAÇÃO FINAL
-- ============================================================
SELECT 'Seed executado com sucesso!' AS status;

COMMIT;
