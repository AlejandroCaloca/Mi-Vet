import React, { useEffect, useState } from "react";
import { Col, Row, Card, ListGroup, Image } from "react-bootstrap";
import PropTypes from "prop-types";
import { Link } from "react-router-dom";
import * as blogService from "services/blogService";
import { formatDate } from "utils/dateFormater";
import toastr from "toastr";
import debug from "sabio-debug";

const _logger = debug.extend("BlogActivity");

toastr.options = {
  toastClass: "analytics-toast-class",
  closeButton: false,
  debug: false,
  newestOnTop: true,
  progressBar: false,
  positionClass: "toast-top-right",
  preventDuplicates: true,
  onclick: null,
  showEasing: "swing",
  hideEasing: "linear",
  showMethod: "fadeIn",
  hideMethod: "fadeOut",
};

const BlogActivity = ({ title }) => {
  const [pageData, setPageData] = useState({
    blogsArray: [],
    pageIndex: 0,
    pageSize: 5,
    totalCount: 0,
  });

  useEffect(() => {
    blogService
      .getPage(pageData.pageIndex, pageData.pageSize)
      .then(onGetBlogSuccess)
      .catch(onGetBlogError);
  }, [pageData.pageIndex, pageData.pageSize]);

  const onGetBlogSuccess = (response) => {
    _logger("GetBlogSuccess response", response);
    setPageData((prevState) => {
      const pd = { ...prevState };
      pd.blogsArray = response.item.pagedItems;
      pd.totalCount = response.item.totalCount;
      return pd;
    });
  };

  const onGetBlogError = (error) => {
    _logger("GetBlogError", error);
    toastr.clear();
    toastr.error("Error");
  };

  const mapActivityData = (item, index) => (
    <ListGroup.Item className="px-0 pt-0 mb-2" key={"blog_" + index}>
      <Row>
        <Col className="col-auto">
          <Link to={`/user/blogs/blogprofile/${item.id}`}>
            <Image
              alt=""
              src={item.imageUrl}
              className="img-fluid rounded img-4by3-lg"
            />
          </Link>
        </Col>
        <Col className="ps-0">
          <Link to={`/user/blogs/blogprofile/${item.id}`}>
            <h5 className="text-primary-hover">{item.title}</h5>
          </Link>
          <p className="mb-1">{item.subject}</p>
          <span className="fs-6">{formatDate(item.dateCreated)}</span>
        </Col>
      </Row>
    </ListGroup.Item>
  );

  return (
    <Card className="flex">
      <Card.Header className="d-flex align-items-center justify-content-between card-header-height">
        <h4 className="mb-0">{title}</h4>
        <Link to="/user/blogs" className="btn btn-outline-white btn-sm">
          View all
        </Link>
      </Card.Header>
      <Card.Body>
        <ListGroup variant="flush">
          {pageData.blogsArray.slice(0).reverse().map(mapActivityData)}
        </ListGroup>
      </Card.Body>
    </Card>
  );
};
export default BlogActivity;

BlogActivity.propTypes = {
  title: PropTypes.string.isRequired,
};
