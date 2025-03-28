import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AdminNavbarPageComponent } from './admin-navbar-page.component';

describe('AdminNavbarPageComponent', () => {
  let component: AdminNavbarPageComponent;
  let fixture: ComponentFixture<AdminNavbarPageComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [AdminNavbarPageComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(AdminNavbarPageComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
