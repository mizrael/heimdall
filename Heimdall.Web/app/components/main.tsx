import * as React from "react";
import * as $ from "jquery";
import { ServicesArchiveRenderer } from "./servicesArchiveRenderer";

export interface MainProps { }
export interface MainState {  }

export class Main extends React.Component<MainProps, MainState> {
    private renderNavbar() {
        return <nav className="navbar navbar-default navbar-fixed-top">
            <div className="container">
                <div className="navbar-header">
                    <h1>Heimdall</h1>
                </div>
            </div>
        </nav>;
    }

    public render() {
        return <div>
            {this.renderNavbar()}
            <ServicesArchiveRenderer />
        </div>;
    }
}