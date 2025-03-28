import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AboutUsFullComponent } from './about-us-full.component';

describe('AboutUsFullComponent', () => {
  let component: AboutUsFullComponent;
  let fixture: ComponentFixture<AboutUsFullComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [AboutUsFullComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(AboutUsFullComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
