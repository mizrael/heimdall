import * as React from "react";
import { Services } from "../services/services";
import { ServiceArchiveItem } from "../models/service";

export interface ServicesArchiveItemRendererState {
    model: ServiceArchiveItem;
    isLoading: boolean;
}

export interface ServicesArchiveItemRendererProps {
    model: ServiceArchiveItem;
    onSelect: Function;
    onDeleted: Function;
    rowIndex: number;
}

export class ServicesArchiveItemRenderer extends React.Component<ServicesArchiveItemRendererProps, ServicesArchiveItemRendererState> {
    constructor(props: ServicesArchiveItemRendererProps) {
        super(props);
        
        this.state = { model: props.model, isLoading: false };
    }
    
    private viewServiceDetails(e:any) {
        e.preventDefault();
        
        this.props.onSelect(this.state.model);
    }

    private refreshService(e: any) {
        e.preventDefault();
        
        let state: ServicesArchiveItemRendererState = this.state,
            provider = new Services();

        if (!state.model) {
            return;
        }

        state.isLoading = true;
        this.setState(state);

        provider.refresh(state.model.name).catch(() => {
            this.onLoadingComplete(state);
        }).then(service => {
            state.model.roundtripTime = Number.MAX_SAFE_INTEGER;
            state.model.endpointsCount = 0;
            state.model.active = false;
            if (service) {
                state.model.active = (null != service.bestEndpoint);
                if (state.model.active) {
                    state.model.roundtripTime = service.bestEndpoint.roundtripTime;
                }
                if (service.endpoints) {
                    state.model.endpointsCount = service.endpoints.length;
                }
            }
            this.onLoadingComplete(state);
        });
    }

    private deleteService(e: any) {
        e.preventDefault();
        
        let state: ServicesArchiveItemRendererState = this.state,
            provider;

        if (!state.model) {
            return;
        }

        state.isLoading = true;
        this.setState(state);

        provider = new Services();
        provider.deleteService(state.model.name).catch(() => {
            this.onLoadingComplete(state);
        }).then(() => {
            this.onLoadingComplete(state);
            this.props.onDeleted(this.props.rowIndex);
        });
    }

    private onLoadingComplete(state: ServicesArchiveItemRendererState) {
        state = state || this.state;
        state.isLoading = false;
        this.setState(state);
    }

    public render() {
        if (!this.state.model) {
            return;
        }

        let actions = (this.state.isLoading) ? <td>processing....</td> :
            <td>
                <ul>
                    <li><button onClick={e => this.viewServiceDetails(e)}>View</button></li>
                    <li><button onClick={e => this.refreshService(e)}>Refresh</button></li>
                    <li><button onClick={e => this.deleteService(e)}>Delete</button></li>
                </ul>
            </td>;

        return <tr>
            <td>{this.state.model.name}</td>
            <td>{this.state.model.active ? "yes" : "no"}</td>
            <td>{this.state.model.endpointsCount}</td>
            <td>{this.state.model.active ? this.state.model.roundtripTime + 's': '-'}</td>
            {actions}
        </tr>;
    }
}