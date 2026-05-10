CREATE TABLE public.users (
                              id uuid NOT NULL,  
                              username character varying(50) NOT NULL,
                              password_hash text NOT NULL,
                              created_at timestamp without time zone NOT NULL
);

CREATE TABLE public.boards (
                               id uuid NOT NULL,  
                               title character varying(100) NOT NULL,
                               description text,
                               sku character varying(3) NOT NULL,
                               owner_id uuid NOT NULL,  
                               created_at timestamp without time zone NOT NULL
);

CREATE TABLE public.sections (
                                 id uuid NOT NULL, 
                                 name character varying(100) NOT NULL,
                                 board_id uuid NOT NULL,  
                                 "position" integer NOT NULL,
                                 is_default boolean DEFAULT false,
                                 created_at timestamp without time zone NOT NULL
);

CREATE TABLE public.tasks (
                              id uuid NOT NULL,  
                              title character varying(200) NOT NULL,
                              description text,
                              board_id uuid NOT NULL,  
                              section_id uuid NOT NULL,  
                              assignee_id uuid,  
                              due_date date,
                              priority character varying(20) DEFAULT 'Medium',
                              is_archived boolean DEFAULT false,
                              created_by uuid NOT NULL,  
                              created_at timestamp without time zone NOT NULL,
                              updated_at timestamp without time zone
);