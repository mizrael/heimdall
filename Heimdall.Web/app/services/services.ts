import "whatwg-fetch";
import { ServiceArchiveItem } from "../models/service";

interface IServices{

}

export class Services implements IServices {
    public readServices(): Promise<Array<ServiceArchiveItem>> { 
        return this.getJsonData<Array<ServiceArchiveItem>>("/api/services");
    }

    private getJsonData<T>(url: string): Promise<T> {
        var headers = new Headers({
            "Content-Type": "application/json"
        });

        var fetchOptions: RequestInit = {
            method: 'GET',
            headers: headers,
            cache: 'default',
        };

        return fetch("/api/services", fetchOptions).then((response: Response) => {
            return response.json<T>().then((data: T) => {
                console.log(data);
                return data;
            });
        });
    }
}