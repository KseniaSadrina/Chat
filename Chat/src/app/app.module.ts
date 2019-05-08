import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './layout/app.component';

import { FormsModule, ReactiveFormsModule } from '@angular/forms'
import { HttpClientModule } from '@angular/common/http';
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
    LoginDialogComponent
  ],
  imports: [
    CustomMaterialModule,
    BrowserModule,
    AppRoutingModule,
    HttpClientModule,
    FormsModule,
    ReactiveFormsModule,
    BrowserAnimationsModule,
    GrowlModule
  ],
  providers: [],
  bootstrap: [AppComponent],
  entryComponents: [AddTrainingComponent, LoginDialogComponent]
})

export class AppModule { }
