import { FormField } from './formField';
import { FormAttachments } from './formAttachments';

export class UserSubmitLoadingForm{
    public form_id:number;
    public user_id:number;
    public locale_id:number;
    public inputs:FormField[] = [];
    // public attachments:FormAttachments[] = [];
    public empty_form:boolean;
    public period:string;
    public year:number;
}