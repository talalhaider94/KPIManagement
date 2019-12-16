import { Component, OnInit } from '@angular/core';
import { pageVersion } from '../../../_page-versions';

@Component({
  selector: 'app-about',
  templateUrl: './about.component.html',
  styleUrls: ['./about.component.scss']
})
export class AboutComponent implements OnInit {

  pageVersions:Object

  constructor() { }

  ngOnInit() {
    this.pageVersions = pageVersion;
  }

}
