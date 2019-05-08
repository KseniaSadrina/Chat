import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { LoginComponent } from './components/login/login.component';
import { TrainingDetailsComponent } from './components/trainings/training-details/training-details.component';
import { PageNotFoundComponent } from './components/page-not-found/page-not-found.component';
import { HomeComponent } from './components/home/home.component';
import { TrainingsListComponent } from './components/trainings/trainings-list/trainings-list.component';
import { TrainingsComponent } from './components/trainings/trainings.component';
import { AddTrainingComponent } from './components/trainings/add-training/add-training.component';

const routes: Routes = [
  { path: '', redirectTo: 'home/trainings', pathMatch: 'full' },
  { path: 'home',
    component: HomeComponent,
    children: [
      { path: 'login', component: LoginComponent },
      { path: 'trainings', component: TrainingsComponent,
        children: [
          {
            path: ':id',
            component: TrainingDetailsComponent
          }
        ]}
    ]
  },
  { path: '**', component: PageNotFoundComponent}
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
