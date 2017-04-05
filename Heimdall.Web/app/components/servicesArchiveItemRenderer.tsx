import * as React from "react";
import * as $ from "jquery";
import { Services } from "../services/services";
import { ServiceArchiveItem } from "../models/service";

export interface ServicesArchiveItemRendererState {
    model: ServiceArchiveItem;
    isLoading: boolean;
}

export interface ServicesArchiveItemRendererProps {
    model: ServiceArchiveItem;
}

export class ServicesArchiveItemRenderer extends React.Component<ServicesArchiveItemRendererProps, ServicesArchiveItemRendererState> {
    constructor(props: ServicesArchiveItemRendererProps) {
        super(props);

        this.state = { model: props.model, isLoading: false };
    }
    
    private viewServiceDetails(e:any) {
        e.preventDefault();

        if (!this.state.model) {
            return;
        }

        let state: ServicesArchiveItemRendererState = this.state,
            provider = new Services();

        state.isLoading = true;
        this.setState(state);

        provider.read(state.model.name).then(service => {
            this.onLoadingComplete(state);

            if (!service)
                return;
        }).catch(() => {
            this.onLoadingComplete(state);
        });
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
            this.onLoadingComplete(state);
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
                <button data-toggle="modal" data-target="#myModal" onClick={e => this.viewServiceDetails(e)}>View</button>
                <button onClick={e => this.refreshService(e)}>Refresh</button>
            </td>;

        return <tr key={this.state.model.name}>
            <td>{this.state.model.name}</td>
            <td>{this.state.model.active ? "yes" : "no"}</td>
            <td>{this.state.model.endpointsCount}</td>
            <td>{this.state.model.roundtripTime}</td>
            {actions}
        </tr>;
    }
}