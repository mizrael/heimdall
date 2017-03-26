import * as React from "react";
import * as $ from "jquery";
import { ServicesArchive } from "./servicesArchive";

export interface MainProps { }
export interface MainState {  }

export class Main extends React.Component<MainProps, MainState> {
    public render() {
        return <ServicesArchive />;
    }
}