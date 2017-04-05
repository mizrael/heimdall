import * as React from "react";
import * as $ from "jquery";
import { ServicesArchiveRenderer } from "./servicesArchiveRenderer";

export interface MainProps { }
export interface MainState {  }

export class Main extends React.Component<MainProps, MainState> {
    public render() {
        return <ServicesArchiveRenderer />;
    }
}