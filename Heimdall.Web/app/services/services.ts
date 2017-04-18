import "whatwg-fetch";
import { ServiceArchiveItem, ServiceDetails, CreateService } from "../models/service";

interface IServices{

}

export class Services implements IServices {
    private static baseUrl: string = "/api/services/";

    public readServices(): Promise<Array<ServiceArchiveItem>> { 
        return this.get<Array<ServiceArchiveItem>>(Services.baseUrl);
    }

    public read(name: string): Promise<ServiceDetails> {
        let url = Services.baseUrl + name;
        return this.get<ServiceDetails>(url);
    }

    public create(model: CreateService): Promise<ServiceDetails> {
        let url = Services.baseUrl;
        return this.post<ServiceDetails>(url, model);
    }

    public refresh(name: string): Promise<ServiceDetails> {
        let url = Services.baseUrl + "refresh/";
        return this.post<ServiceDetails>(url, name);
    }

    public deleteService(name: string): Promise<void> {
        let url = Services.baseUrl;
        return this.delete<void>(url, name);
    }

    private post<T>(url: string, data: any = null): Promise<T> {
        return this.executeRequest(url, "POST", data);
    }

    private delete<T>(url: string, data: any = null): Promise<T> {
        return this.executeRequest(url, "DELETE", data);
    }

    private put<T>(url: string, data: any = null): Promise<T> {
        return this.executeRequest(url, "PUT", data);
    }

    private get<T>(url: string): Promise<T> {
        return this.executeRequest(url, "GET");
    }

    private executeRequest<T>(url: string, method:string, data:any = null): Promise<T> {
        var fetchOptions = this.buildRequest(method, data);

        return fetch(url, fetchOptions)
            .catch(reason => {
                console.log(reason);
                return null;
            })
            .then((response: Response) => {
            if (!response.ok)
                return null;
            return response.json().then<T>((data: T) => {
                return data;
            });
        });
    }

    private buildRequest(method: string, data: any = null): RequestInit {
        let headers = new Headers({
            "Content-Type": "application/json"
        }),
            body = null;

        if (data) {
            body = JSON.stringify(data);
        }

        var fetchOptions: RequestInit = {
            method: method,
            headers: headers,
            cache: 'default',
            body: body
        };
        return fetchOptions;
    }
}