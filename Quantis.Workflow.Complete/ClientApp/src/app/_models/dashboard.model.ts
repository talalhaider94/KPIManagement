export interface WidgetModel {
    name: string;
    uiidentifier: string;
    createdon: Date;
    help: string;
    iconurl: string;
    id: number;
    url: string;
    widgetcategoryname: string;
    wigetcategoryid: number;
}

export interface DashboardContentModel {
    cols: number;
    rows: number;
    y: number;
    x: number;
    component?: any;
    widgetname: string;
    filters: {};
    properties: {};
    widgetid: number;
    dashboardid: number;
    id: number;
    uiidentifier: string;
    url?: string;
    minItemRows?: number;
    minItemCols?: number;
}

export interface DashboardModel {
    id: number;
    name: string;
    owner: string;
    createdon: Date;
    globalfilterid: number,
    dashboardwidgets: Array<DashboardContentModel>;
    isactive: boolean;
}
