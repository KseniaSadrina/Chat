import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './layout/app.component';

import { FormsModule, ReactiveFormsModule } from '@angular/forms'
import { HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';
import {BrowserAnimationsModule} from '@angular/platform-browser/animations';
import { CustomMaterialModule } from 'src/app/custom-material.module';
import { HeadComponent } from './layout/head/head.component';
import { SessionComponent } from './components/session/session.component';
import { LoginComponent } from './components/login/login.component';
import { TrainingsListComponent } from './components/trainings/trainings-list/trainings-list.component';
import { TrainingDetailsComponent } from './components/trainings/training-details/training-details.component';
import { AddTrainingComponent } from './components/trainings/add-training/add-training.component';
import { GrowlModule } from 'primeng/primeng';
import { PageNotFoundComponent } from './components/page-not-found/page-not-found.component';
import { HomeComponent } from './components/home/home.component';
import { TrainingsComponent } from './components/trainings/trainings.component';
import { LoginDialogComponent } from './components/login/login-dialog/login-dialog.component';
import { SocialLoginModule,
         AuthServiceConfig,
         GoogleLoginProvider, LinkedInLoginProvider, FacebookLoginProvider } from 'angularx-social-login';
import { RegisterComponent } from './components/register/register.component';
import { RegisterDialogComponent } from './components/register/register-dialog/register-dialog.component';
import { JwtInterceptor } from './helpers/jwt-interceptor';
import { MockTrainingComponent } from './components/trainings/mock-training/mock-training.component';


const config = new AuthServiceConfig([
  {
    id: GoogleLoginProvider.PROVIDER_ID,
    provider: new GoogleLoginProvider('Google-OAuth-Client-Id')
  },
  {
    id: FacebookLoginProvider.PROVIDER_ID,
    provider: new FacebookLoginProvider('Facebook-App-Id')
  },
  {
    id: LinkedInLoginProvider.PROVIDER_ID,
    provider: new LinkedInLoginProvider('LinkedIn-client-Id', false, 'en_US')
  }
]);

@NgModule({
  declarations: [
    AppComponent,
    HeadComponent,
    SessionComponent,
    LoginComponent,
    TrainingsListComponent,
    TrainingDetailsComponent,
    AddTrainingComponent,
    PageNotFoundComponent,
    HomeComponent,
    TrainingsComponent,
    LoginDialogComponent,
    RegisterComponent,
    RegisterDialogComponent,
    MockTrainingComponent
  ],
  imports: [
    CustomMaterialModule,
    BrowserModule,
    AppRoutingModule,
    HttpClientModule,
    FormsModule,
    ReactiveFormsModule,
    BrowserAnimationsModule,
    GrowlModule,
    SocialLoginModule
  ],
  providers: [
    { provide: AuthServiceConfig, useFactory: config },
    { provide: HTTP_INTERCEPTORS, useClass: JwtInterceptor, multi: true },
  ],
  bootstrap: [AppComponent],
  entryComponents: [AddTrainingComponent, LoginDialogComponent, RegisterDialogComponent]
})

export class AppModule { }
