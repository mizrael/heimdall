export class ServiceArchiveItem{
    name: string;
    active: boolean;
    endpointsCount: number;
    roundtripTime: number;
}

export class ServiceDetails {
    name: string;
    endpoints: Array<ServiceEndpoint>;
}

export class ServiceEndpoint {
    url: string;
    active: boolean;
    roundtripTime: number;
}