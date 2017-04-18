export class ServiceArchiveItem{
    name: string;
    active: boolean;
    endpointsCount: number;
    roundtripTime: number;
}

export class ServiceDetails {
    name: string;
    active: boolean;
    endpoints: Array<ServiceEndpoint>;
    bestEndpoint: ServiceEndpoint;
}

export class ServiceEndpoint {
    url: string;
    active: boolean;
    roundtripTime: number;
}

export class CreateService {
    name: string;
    endpoint: string;
}