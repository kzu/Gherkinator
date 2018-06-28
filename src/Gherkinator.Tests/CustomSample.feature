Feature: Sample
  A sample feature showing how to run the file from xunit

  Background:
    Given Foo=baz

  Scenario: Foo should equal Bar
    Given Bar=baz
    Then foo equals bar