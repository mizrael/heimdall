import { Component, Input  } from '@angular/core';
import { IApiErrorResult } from '../../models/common';

@Component({
    selector: 'errors',
    templateUrl: './errors.component.html'
})
export class ErrorsComponent {
   @Input() model: IApiErrorResult | null;
}