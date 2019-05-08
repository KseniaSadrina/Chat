import { Component, OnInit } from '@angular/core';
import { Observable } from 'rxjs';
import { CustomAuthService } from 'src/app/services/custom-auth.service';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css']
})
export class HomeComponent implements OnInit {

  constructor(private authService: CustomAuthService) { }
  isLoggingIn: Observable<boolean>;

  ngOnInit() {
    this.isLoggingIn = this.authService.isLoggingIn$;
  }

}
