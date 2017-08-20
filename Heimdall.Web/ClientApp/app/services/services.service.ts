import { Inject, Injectable } from "@angular/core";
import { Http } from "@angular/http";
import { IServiceArchiveItem, IServiceDetails } from "../models/service";
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
}