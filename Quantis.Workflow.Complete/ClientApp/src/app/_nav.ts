interface NavAttributes {
    [propName: string]: any;
}
interface NavWrapper {
    attributes: NavAttributes;
    element: string;
}
interface NavBadge {
    text: string;
    variant: string;
}
interface NavLabel {
    class?: string;
    variant: string;
}

export interface NavData {
    name?: string;
    key?: any;
    url?: string;
    icon?: string;
    badge?: NavBadge;
    title?: boolean;
    version?: string;
    children?: NavData[];
    variant?: string;
    attributes?: NavAttributes;
    divider?: boolean;
    class?: string;
    label?: NavLabel;
    wrapper?: NavWrapper;
    UIVersion?: boolean;
}

export const navItems: NavData[] = [
    {
        title: true,
        name: 'Menu',
        key: 'alwaysShow'
    },
    {
        name: 'Dashboard',
        url: '/dashboard/list',
        icon: 'fa fa-laptop',
        key: 'alwaysShow',
        version: '1.0.0',
    },
    {
        name: 'Workflow',
        url: '/workflow-menu',
        icon: 'fa fa-code-fork',
        key: ['VIEW_WORKFLOW_KPI_VERIFICA', 'VIEW_WORKFLOW_RICERCA', 'VIEW_WORKFLOW_ADMIN','VIEW_WORKFLOW_MONITORING_ORG','VIEW_WORKFLOW_MONITORING_DAY'],
        children: [
            {
                name: 'KPI in Verifica',
                url: '/workflow/verifica',
                icon: 'fa fa-file-text-o',
                version: '0.1.5',
                key: 'VIEW_WORKFLOW_KPI_VERIFICA'
            },
            {
                name: 'Ricerca',
                url: '/workflow/ricerca',
                icon: 'fa fa-search',
                version: '0.1.5',
                key: ['VIEW_WORKFLOW_RICERCA']
            },
            {
                name: 'Amministrazione',
                url: '/workflow/amministrazione',
                icon: 'fa fa-users',
                version: '0.0.2',
                key: ['VIEW_WORKFLOW_ADMIN']
            },
            {
                name: 'Stato',
                url: '/workflow/stato',
                icon: 'fa fa-users',
                version: '0.0.1',
                key: ['VIEW_WORKFLOW_MONITORING_ORG']
            },
            // {
            //     name: 'Stato',
            //     url: '/workflow/statoDay',
            //     icon: 'fa fa-users',
            //     version: '0.0.1',
            //     key: ['VIEW_WORKFLOW_MONITORING_DAY']
            // }
        ]
    },
    {
        name: 'Catalogo',
        url: '/catalogo',
        icon: 'fa fa-folder-open-o',
        key: ['VIEW_CATALOG_KPI', 'VIEW_CATALOG_UTENTI', 'VIEW_UTENTI_DA_CONSOLIDARE', 'VIEW_KPI_DA_CONSOLIDARE'],
        children: [
            {
                name: 'KPI da Consolidare',
                url: '/catalogo/admin-kpi',
                icon: 'fa fa-file-archive-o',
                version: '0.0.1',
                key: 'VIEW_KPI_DA_CONSOLIDARE'
            },
            {
                name: 'Utenti da Consolidare',
                url: '/catalogo/admin-utenti',
                icon: 'fa fa-address-book-o',
                version: '0.0.1',
                key: 'VIEW_UTENTI_DA_CONSOLIDARE'
            },
            {
                name: 'Catalogo KPI',
                url: '/catalogo/kpi',
                icon: 'fa fa-file-archive-o',
                version: '0.0.4',
                key: 'VIEW_CATALOG_KPI'
            },
            {
                name: 'Catalogo Utenti',
                url: '/catalogo/utenti',
                icon: 'fa fa-address-book-o',
                version: '0.0.1',
                key: 'VIEW_CATALOG_UTENTI',
            },
        ]
    },
    {
        name: 'KPI Certificati',
        url: '/archivedkpi',
        icon: 'fa fa-archive',
        version: '0.0.4',
        key: 'VIEW_KPI_CERTICATI',
    },
    {
        name: 'Loading Form',
        url: '/loadingform-menu',
        icon: 'fa fa-pencil-square-o',
      key: ['VIEW_ADMIN_LOADING_FORM', 'VIEW_LOADING_FORM_UTENTI', 'VIEW_LOADING_CSV'],
        children: [
            {
                name: 'Admin',
                url: '/loading-form/admin',
                icon: 'fa fa-user-circle',
                version: '0.0.1',
                key: 'VIEW_ADMIN_LOADING_FORM'
            },
            {
                name: 'S-Utente',
                url: '/loading-form/securityuser',
                icon: 'fa fa-user-circle-o',
                version: '0.0.12',
                key: 'VIEW_ADMIN_LOADING_FORM'
            },
            {
                name: 'LF da Compilare',
                url: '/loading-form/utente',
                icon: 'fa fa-user-circle-o',
                version: '0.0.12',
                key: 'VIEW_LOADING_FORM_UTENTI'
            },
            {
                name: 'LF Fuori Periodo',
                url: '/loading-form/utente-notracking',
                icon: 'fa fa-user-circle-o',
                version: '0.0.12',
                key: 'VIEW_LOADING_FORM_UTENTI'
            },
            {
              name: 'CSV da inviare',
              url: '/loading-form/utente-csv',
              icon: 'fa fa-user-circle-o',
              version: '0.0.12',
              key: 'VIEW_LOADING_CSV'
            },
        ]
    },
    {
        name: 'Report',
        url: '/report',
        icon: 'fa fa-bar-chart',
      key: ['VIEW_NOTIFIER_EMAILS', 'VIEW_RAW_DATA', 'VIEW_BOOKLET', 'VIEW_BOOKLET_FROM_TEMPLATE', 'VIEW_FREE_FORM_REPORT', 'VIEW_REPORT_FROM_BSI', 'VIEW_REPORT_CUSTOM'],
        children: [ 
            {
                name: 'Notifiche LoadingForm',
                url: '/specialreporting',
                icon: 'fa fa-envelope',
                version: '0.0.4',
                key: 'VIEW_NOTIFIER_EMAILS'
            },
            {
                name: 'Dati Grezzi',
                url: '/datigrezzi',
                icon: 'fa fa-copy',
                version: '0.0.1',
                key: 'VIEW_RAW_DATA'
            },
            {
                name: 'Booklet',
                url: '/general-booklet',
                icon: 'fa fa-book',
                version: '0.0.1',
                key: 'VIEW_BOOKLET'
            },
            {
                name: 'Booklet da Template',
                url: '/booklet',
                icon: 'fa fa-book',
                version: '0.0.1',
                key: 'VIEW_BOOKLET_FROM_TEMPLATE'
            },
            {
                name: 'Free Form Report',
                url: '/formreport',
                icon: 'fa fa-book',
                version: '0.0.1',
                key: 'EDIT_FREEFORM_REPORT'
            },
            {
                name: 'Free Form Report',
                url: '/dashboard/free-form-report',
                icon: 'fa fa-file-text-o',
                version: '0.0.1',
                key: 'VIEW_FREE_FORM_REPORT'
            },
            {
                name: 'Report da BSI',
                url: '/dashboard/bsi-report',
                icon: 'fa fa-file-text-o',
                version: '0.0.1',
                key: 'VIEW_REPORT_FROM_BSI'
            },
            {
                name: 'Report personali',
                url: '/dashboard/personal-report',
                icon: 'fa fa-file-text-o',
                version: '0.0.1',
              key: 'VIEW_REPORT_CUSTOM'
            }
        ]
    },
    {
        name: 'Configurazione',
        url: '/config-menu',
        icon: 'fa fa-gear',
        key: ['VIEW_CONFIGURATION_GENERAL', 'VIEW_CONFIGURATION_ADVANCED', 'VIEW_CONFIGURATION_SDM_GROUP',
            'VIEW_CONFIGURATION_SDM_TICKET_STATUS', 'VIEW_CONFIGURATION_ROLES', 'VIEW_CONFIGURATION_USER_ROLES',
            'VIEW_CONFIGURATION_USER_PROFILING', 'VIEW_CONFIGURATION_STANDARD_DASHBOARD', 'IMPORT_FREE_FORM_REPORT',
            'VIEW_CUTOFF_WORKFLOW_DAY','VIEW_CONFIGURATION_ADVANCED_2'
        ],
        children: [
            {
                name: 'Generali',
                url: '/tconfiguration/general',
                icon: 'fa fa-check-circle-o',
                version: '0.0.2',
                key: 'VIEW_CONFIGURATION_GENERAL'
            },
            {
                name: 'Avanzate',
                url: '/tconfiguration/advanced',
                icon: 'fa fa-check-circle-o',
                version: '0.0.2',
                key: 'VIEW_CONFIGURATION_ADVANCED'
            },
            {
                name: 'SDM Gruppi',
                url: '/sdmgroup',
                icon: 'fa fa-gear',
                version: '0.0.1',
                key: 'VIEW_CONFIGURATION_SDM_GROUP'
            },
            {
                name: 'SDM Ticket Status',
                url: '/sdmstatus',
                icon: 'fa fa-gear',
                version: '0.0.1',
                key: 'VIEW_CONFIGURATION_SDM_TICKET_STATUS'
            },
            {
                name: 'Gestione Ruoli',
                url: '/adminroles',
                icon: 'fa fa-gear',
                version: '0.0.1',
                key: 'VIEW_CONFIGURATION_ROLES'
            },
            {
                name: 'Ruoli Utente',
                url: '/userprofiling/rolepermissions',
                icon: 'fa fa-gear',
                version: '0.0.1',
                key: 'VIEW_CONFIGURATION_USER_ROLES'
            },
            {
                name: 'Profilazione Utente',
                url: '/userprofiling/userpermissions',
                icon: 'fa fa-gear',
                version: '0.0.6',
                key: 'VIEW_CONFIGURATION_USER_PROFILING'
            },
            {
                name: 'Standard Dashboard',
                url: '/standarddashboard',
                icon: 'fa fa-gear',
                version: '0.0.1',
                key: 'VIEW_CONFIGURATION_STANDARD_DASHBOARD'
            },
            {
                name: 'Import Free Form Report',
                url: '/dashboard/import-form-report',
                icon: 'fa fa-file-text-o',
                version: '0.0.1',
                key: 'IMPORT_FREE_FORM_REPORT'
            },
            {
                name: 'Workflow U.O.',
                url: '/workfloworganizationunit/workflowunit',
                icon: 'fa fa-file-text-o',
                version: '0.0.1',
                key: 'VIEW_CUTOFF_WORKFLOW_DAY'
            },
            {
                name: 'Speciali',
                url: '/tconfiguration/organization',
                icon: 'fa fa-check-circle-o',
                version: '0.0.1',
                key: 'VIEW_CONFIGURATION_ADVANCED_2'
            }
            /*{
                name: 'Workflow',
                url: '/tconfiguration/workflow',
                icon: 'fa fa-file-text-o',
                version: '0.0.1',
                key: 'VIEW_CUTOFF_WORKFLOW_DAY'
            }*/
        ]
    },
    // {
    //     name: 'Power BI',
    //     url: "window.open('www.google.com', '_blank')",
    //     icon: 'fa fa-archive',
    //     version: '0.0.1',
    //     key: 'VIEW_EXTERNAL_POWERBI',
    // },
    {
        divider: true,
        key: 'alwaysShow'
    },
    {
        title: true,
        name: 'UIVersion',
        UIVersion: true,
        class: 'class-version-nav',
        key: 'alwaysShow'
    },
];
