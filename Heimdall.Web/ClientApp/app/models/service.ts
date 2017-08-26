export interface IServiceArchiveItem {
    name: string;
    active: boolean;
    endpointsCount: number;
    roundtripTime: number;
}

export interface IServiceEndpoint {
    address: string;
    protocol: string;
    active: boolean;
    roundtripTime: number;
}

export interface IServiceDetails {
    name: string;
    active: boolean;
    endpoints: IServiceEndpoint[];
    bestEndpoint: IServiceEndpoint;
}