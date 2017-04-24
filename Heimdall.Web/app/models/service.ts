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
    constructor(name: string, endpoint: string) {
        this.name = name;
        this.endpoint = endpoint;
    }

    readonly name: string;
    readonly endpoint: string;
}

export class AddEndpoint {
    constructor(serviceName: string, endpoint: string) {
        this.serviceName = serviceName;
        this.endpoint = endpoint;
    }

    readonly serviceName: string;
    readonly endpoint: string;
}

export class RemoveEndpoint {
    constructor(serviceName: string, endpoint: string) {
        this.serviceName = serviceName;
        this.endpoint = endpoint;
    }

    readonly serviceName: string;
    readonly endpoint: string;
}