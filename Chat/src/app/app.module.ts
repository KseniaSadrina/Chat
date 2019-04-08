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
import { LogoutComponent } from './components/logout/logout.component';
import { TrainingsListComponent } from './components/trainings-list/trainings-list.component';
import { TrainingDetailsComponent } from './components/training-details/training-details.component';
import { AddTrainingComponent } from './components/add-training/add-training.component';
import { GrowlModule } from 'primeng/primeng';

@NgModule({
  declarations: [
    AppComponent,
    HeadComponent,
    SessionComponent,
    LoginComponent,
    LogoutComponent,
    TrainingsListComponent,
    TrainingDetailsComponent,
    AddTrainingComponent
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
  entryComponents: [AddTrainingComponent]
})

export class AppModule { }
