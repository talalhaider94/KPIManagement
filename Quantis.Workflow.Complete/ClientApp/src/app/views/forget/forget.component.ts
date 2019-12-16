import { Component, OnInit } from '@angular/core';
import { FormGroup, FormBuilder, Validators } from '@angular/forms';
import { AuthService } from '../../_services';
import { ToastrService } from 'ngx-toastr';
import { first } from 'rxjs/operators';
import { Router } from '@angular/router';

@Component({
    selector: 'app-dashboard',
    templateUrl: 'forget.component.html'
})

export class ForgetComponent implements OnInit {
    title = 'Forget?';
    forgetForm: FormGroup;
    submitted: boolean = false;
    loading: boolean = false;

    constructor(
        private formBuilder: FormBuilder,
        private authService: AuthService,
        private toastr: ToastrService,
        private router: Router
    ) {
        if (this.authService.currentUserValue || this.authService.isLoggedIn()) {
            this.router.navigate(['/dashboard']);
        }
    }

    get f() { return this.forgetForm.controls; }

    ngOnInit() {
        this.forgetForm = this.formBuilder.group({
            userName: ['', Validators.required],
            email: ['', [Validators.required, Validators.email]],
        });
    }

    onForgetFormSubmit() {
        this.submitted = true;
        if (this.forgetForm.invalid) {
            this.toastr.error('Compilare tutti i campi del form.', 'Error');
            return;
        } else {
            const { userName, email } = this.f;
            this.loading = true;
            this.authService.resetPassword(userName.value, email.value).pipe(first()).subscribe(result => {
                this.loading = false;
                console.log(result)
                if (!!result) {
                    this.router.navigate(['/login']);
                    this.toastr.success('Password inviata all\'email indicata.', 'Success');
                } else {
                    this.toastr.error('Username o email errati.', 'Error');
                }
            }, error => {
                console.error('onForgetFormSubmit', error)
                this.toastr.error('Errore durante la creazione della password.', 'Error');
                this.loading = false;
            })
        }
    }
}
