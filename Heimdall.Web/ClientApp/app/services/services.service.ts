import { Inject, Injectable } from "@angular/core";
import { Http, RequestOptionsArgs } from "@angular/http";
import { IServiceArchiveItem, IServiceDetails, IAddEndpoint, IDeleteEndpoint } from "../models/service";
import 'rxjs/add/operator/map';
import 'rxjs/add/operator/toPromise';

@Injectable()
export class ServicesService {
    private serviceUrl: string;

    constructor( @Inject("BASE_URL") baseUrl: string, private http: Http) {
        this.serviceUrl = baseUrl + "api/services/";
    }

    public getAll(): Promise<IServiceArchiveItem[]> {
        return this.http.get(this.serviceUrl)
            .map(response => response.json() as IServiceArchiveItem[])
            .toPromise();
    }

    public get(name: string): Promise<IServiceDetails> {
        let url = this.serviceUrl + name;

        return this.http.get(url)
            .map(response => response.json() as IServiceDetails)
            .toPromise();
    }

    public addEndpoint(dto: IAddEndpoint): Promise<boolean> {
        let data: RequestOptionsArgs = {
            body: dto
        };
        return this.http.post(this.serviceUrl, dto)
            .map(response => response.ok)
            .toPromise();
    }

    public deleteEndpoint(dto: IDeleteEndpoint): Promise<boolean> {
        let url = this.serviceUrl + 'endpoint',
            data: RequestOptionsArgs = {
            body: dto
        };
        return this.http.delete(url, data)
            .map(response => response.ok)
            .toPromise();
    }
}